import typing
from io import BytesIO
from uuid import UUID

from clients.s3_client import S3Client
from common.file_util import FileUtil
from pg_adapter import Adapter
from postgres.models.db_models import Chat, File, FileSender
from telegram_client_loader.model.telegram_file import TelegramFile
from telethon import TelegramClient
from telethon.events import NewMessage
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument


class TelegramLoader:
    telegram_client: TelegramClient
    adapter: Adapter
    s3_client: S3Client

    def __init__(self, telegram_client: TelegramClient, adapter: Adapter, s3_client: S3Client):
        self.telegram_client = telegram_client
        self.adapter = adapter
        self.s3_client = s3_client

    async def run(self):
        message: NewMessage = NewMessage()
        self.telegram_client.add_event_handler(
            self.__handle_new_message, message)

    async def __handle_new_message(self, event: NewMessage.Event):
        message: Message = event.message
        is_valid = await self.__is_valid_chat(message.chat_id)

        if is_valid and message.media:
            telegram_file: TelegramFile = self.__get_telegram_file(message)
            file_info = await self.__save_file_info(telegram_file)

            await self.__upload_file_to_storage(message=message, filename=telegram_file.filename, key=file_info.Id)

    async def __is_valid_chat(self, chat_id) -> bool:
        return await self.adapter.contains(model=Chat, TelegramId=chat_id)

    @staticmethod
    def __get_telegram_file(message: Message):
        chat_id = message.chat_id
        sender_id = message.sender.id

        file_type, filename, extension = FileUtil.get_document_file_info(message.media) \
            if isinstance(message.media, MessageMediaDocument) \
            else FileUtil.get_photo_file_info(message)

        telegram_file = TelegramFile(
            chat_id=chat_id,
            sender_id=sender_id,
            filename=filename,
            extension=extension,
            file_type=file_type
        )

        return telegram_file

    async def __save_file_info(self, telegram_file: TelegramFile) -> File:
        file_sender = await self.adapter.get(FileSender, TelegramId=telegram_file.sender_id)
        chat = await self.adapter.get(Chat, TelegramId=telegram_file.chat_id)
        file_info = telegram_file.to_file(chat.Id, file_sender.Id)

        return await self.adapter.create(File, **file_info.dict(by_alias=True))

    async def __upload_file_to_storage(self, message: Message, filename: str, key: UUID):
        file = await self.__download_file(message)
        await self.s3_client.upload_file(key=str(key), file=file, filename=filename)

    async def __download_file(self, message: Message) -> BytesIO:
        file: BytesIO = typing.cast(BytesIO, await self.telegram_client.download_media(message, file=BytesIO()))
        return BytesIO(file.getvalue())
