from io import BytesIO

from clients.search_documents_api_client import SearchDocumentsClient
from common.file_util import FileUtil
from common.interactor.chat_interactor import ChatInteractor
from common.interactor.loader_interactor import LoaderInteractor
from postgres.models.external_models import File, FileTypeEnum
from pydantic import AnyUrl
from telegram_client_loader.handler.base_handler import BaseHandler
from telethon import TelegramClient
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument, User


class FileHandler(BaseHandler):

    def __init__(
            self,
            telegram_client: TelegramClient,
            loader_interactor: LoaderInteractor,
            chat_interactor: ChatInteractor,
            search_document_client: SearchDocumentsClient,
    ):
        super(FileHandler, self).__init__(
            telegram_client, loader_interactor
        )

        self.loader_interactor = loader_interactor
        self.chat_interactor = chat_interactor
        self.search_document_client = search_document_client
        self.run()

    async def _handle_new_message_with_media(self, message: Message):
        telegram_file: File = self.__get_telegram_file(message)

        me = await self.telegram_client.get_me()
        filtered_users: list[User] = await self._get_users_without_me(message.chat, me.id)
        await self.chat_interactor.add_new_users(filtered_users, telegram_file.chat_telegram_id)

        file: BytesIO = await self._download_file(message)
        uuid = await self.loader_interactor.save_file(telegram_file, file)

        if telegram_file.type is FileTypeEnum.TextDocument:
            await self.search_document_client.index_document(document_id=uuid, name=telegram_file.name, content=file)

    @staticmethod
    def __get_telegram_file(message: Message) -> File:
        chat_id = message.chat_id
        sender_id = message.sender.id

        file_type, filename, extension = FileUtil.get_document_file_info(message.media) \
            if isinstance(message.media, MessageMediaDocument) \
            else FileUtil.get_photo_file_info(message)

        telegram_file = File(
            name=filename,
            extension=extension,
            type=file_type,
            upload_date=message.date,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id,
        )

        return telegram_file

    async def _handle_new_message_with_urls(self, message: Message, urls: list[AnyUrl]):
        for url in urls:
            await self.loader_interactor.save_url(url, message.sender_id, message.chat_id)

    async def _handle_new_message_with_tags(self, message: Message, marked_texts: (str, str)):
        title, description = marked_texts

        if self.is_empty(title) or self.is_empty(description):
            return

        await self.loader_interactor.save_text(title, description, message.sender_id, message.chat_id)

    @staticmethod
    def is_empty(text: str) -> bool:
        return not text or text.isspace()
