import typing
from io import BytesIO

from common.file_util import FileUtil
from common.interactor.loader_interactor import LoaderInteractor
from telegram_client_loader.model.telegram_file import TelegramFile
from telethon import TelegramClient
from telethon.events import NewMessage
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument


class TelegramLoader:
    telegram_client: TelegramClient
    loader_interactor: LoaderInteractor

    def __init__(
        self,
        telegram_client: TelegramClient,
        loader_interactor: LoaderInteractor
    ):
        self.telegram_client = telegram_client
        self.loader_interactor = loader_interactor
        self.run()

    def run(self):
        message: NewMessage = NewMessage()
        self.telegram_client.add_event_handler(
            self.__handle_new_message, message)

    def stop(self):
        self.telegram_client.remove_event_handler(self.__handle_new_message)

    async def __handle_new_message(self, event: NewMessage.Event):
        message: Message = event.message
        is_valid = await self.__is_valid_chat(message.chat_id)

        if is_valid and message.media:
            telegram_file: TelegramFile = self.__get_telegram_file(message)
            file: BytesIO = await self.__download_file(message)
            await self.loader_interactor.save_file(telegram_file, file)

    async def __is_valid_chat(self, chat_id: int) -> bool:
        return await self.loader_interactor.is_valid_chat(chat_id)

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

    async def __download_file(self, message: Message) -> BytesIO:
        file: BytesIO = typing.cast(BytesIO, await self.telegram_client.download_media(message, file=BytesIO()))
        return BytesIO(file.getvalue())
