import typing
import uuid

from io import BytesIO
from telethon import TelegramClient
from telethon.events import NewMessage
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument, DocumentAttributeFilename
from clients.s3_client import S3Client
from pg_adapter import Adapter
from postgres.models.db_models import Chat, FileSender, File
from telegram_loader.model.TelegramFile import TelegramFile


class TelegramLoader:
    PHOTO_DATE_FORMAT = '%m-%d-%Y_%H-%M-%S'
    DEFAULT_PHOTO_EXTENSION = 'jpg'
    DEFAULT_PHOTO_MIME_TYPE = "image"

    telegram_client: TelegramClient
    adapter: Adapter
    s3_client: S3Client

    def __init__(self, telegram_client: TelegramClient, adapter: Adapter, s3_client: S3Client):
        self.telegram_client = telegram_client
        self.adapter = adapter
        self.s3_client = s3_client

    async def run(self):
        message: NewMessage = NewMessage()

        self.telegram_client.add_event_handler(self.handle_new_message, message)

    async def is_valid_chat(self, chat_id) -> bool:
        return await self.adapter.contains(model=Chat, TelegramId=chat_id)

    async def handle_new_message(self, event: NewMessage.Event):
        message: Message = event.message
        is_valid = await self.is_valid_chat(message.chat_id)

        if is_valid and message.media:
            telegram_file: TelegramFile = await self.get_telegram_file(message)

            self.save_file_info(telegram_file)
            self.upload_file_to_storage(telegram_file.filename, message)

    async def get_telegram_file(self, message: Message):
        chat_id = message.chat_id
        sender_id = message.sender.id

        file_type, filename, extension = self.get_document_file_info(message.media) \
            if isinstance(message.media, MessageMediaDocument) \
            else self.get_photo_file_info(message)

        telegram_file = TelegramFile(
            chat_id=chat_id,
            sender_id=sender_id,
            filename=filename,
            extension=extension,
            file_type=file_type
        )

        return telegram_file

    def get_document_file_info(self, media: MessageMediaDocument) -> (str, str, str):
        document = media.document
        file_type = self.get_file_type(document.mime_type)
        filename_attribute = self.find_filename_attribute(document.attributes)
        filename = f'{uuid.uuid4()}_{filename_attribute.file_name}'
        extension = filename.split('.')[-1]

        return file_type, filename, extension

    def get_photo_file_info(self, message: Message) -> (str, str, str):
        file_type = self.DEFAULT_PHOTO_MIME_TYPE
        filename: str = f'{uuid.uuid4()}_photo_{message.date.strftime(self.PHOTO_DATE_FORMAT)}.' \
                        f'{self.DEFAULT_PHOTO_EXTENSION}'
        extension = self.DEFAULT_PHOTO_EXTENSION

        return file_type, filename, extension

    @staticmethod
    def find_filename_attribute(attributes):
        return [attribute for attribute in attributes if isinstance(attribute, DocumentAttributeFilename)][-1]

    @staticmethod
    def get_file_type(mime_type: str) -> str:
        telegram_file_type = mime_type.split('/')[0]

        if telegram_file_type == 'application' or telegram_file_type == 'text':
            return 'document'

        return telegram_file_type

    def save_file_info(self, telegram_file: TelegramFile):
        file_sender = await self.adapter.get(FileSender, TelegramId=telegram_file.sender_id)
        chat = await self.adapter.get(Chat, TelegramId=telegram_file.chat_id)

        file_info = telegram_file.to_file(chat.Id, file_sender.Id)
        await self.adapter.create(File, **file_info.dict(by_alias=True))

    def upload_file_to_storage(self, filename: str, message: Message):
        file = await self.download_file(message)
        await self.s3_client.upload_file(key=filename, file=file)

    async def download_file(self, message: Message) -> BytesIO:
        file: BytesIO = typing.cast(BytesIO, await self.telegram_client.download_media(message, file=BytesIO()))

        return BytesIO(file.getvalue())
