import typing
from io import BytesIO
from telethon import TelegramClient
from telethon.events import NewMessage
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument, DocumentAttributeFilename
from telegram_loader.model.TelegramFile import TelegramFile


class TelegramLoader:
    PHOTO_DATE_FORMAT = '%m-%d-%Y_%H-%M-%S'
    DEFAULT_PHOTO_EXTENSION = 'jpg'
    DEFAULT_PHOTO_MIME_TYPE = "image"

    telegram_client: TelegramClient

    def __init__(self, telegram_client: TelegramClient):
        self.telegram_client = telegram_client

    async def run(self):
        chats = await self.get_chats()
        message: NewMessage = NewMessage(chats=chats)

        self.telegram_client.add_event_handler(self.handle_new_message, message)

    async def get_chats(self) -> list[int]:
        # TODO: Get chats from db
        pass

    async def handle_new_message(self, event: NewMessage.Event):
        message: Message = event.message

        if message.media:
            telegram_file: TelegramFile = await self.get_telegram_file(message)
            print(telegram_file)
            # TODO: get sender from db and save file info to db
            # TODO: save file to s3

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
        filename = filename_attribute.file_name
        extension = filename.split('.')[-1]

        return file_type, filename, extension

    def get_photo_file_info(self, message: Message) -> (str, str, str):
        file_type = self.DEFAULT_PHOTO_MIME_TYPE
        filename: str = f'photo_{message.date.strftime(self.PHOTO_DATE_FORMAT)}.{self.DEFAULT_PHOTO_EXTENSION}'
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

    async def download_file(self, message: Message) -> bytes:
        file: BytesIO = typing.cast(
            BytesIO,
            await self.telegram_client.download_media(
                message,
                file=BytesIO(),
                progress_callback=lambda downloaded_size, size: print(f'progress: {downloaded_size / size * 100}')
            )
        )

        return file.getvalue()
