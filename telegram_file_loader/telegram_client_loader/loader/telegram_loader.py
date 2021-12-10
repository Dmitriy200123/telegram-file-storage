import re
from datetime import datetime
from io import BytesIO
from urllib.error import URLError
from urllib.request import urlopen

from bs4 import BeautifulSoup
from common.file_util import FileUtil
from common.interactor.chat_interactor import ChatInteractor
from common.interactor.loader_interactor import LoaderInteractor
from postgres.models.db_models import FileTypeEnum
from postgres.models.external_models import File
from postgres.models.external_models.file import type_map
from telegram_client_loader.handler.base_handler import BaseHandler
from telethon import TelegramClient
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument, User


class TelegramLoader(BaseHandler):

    def __init__(
        self,
        telegram_client: TelegramClient,
        loader_interactor: LoaderInteractor,
        chat_interactor: ChatInteractor
    ):
        super(TelegramLoader, self).__init__(
            telegram_client, loader_interactor
        )

        self.loader_interactor = loader_interactor
        self.chat_interactor = chat_interactor
        self.run()

    async def _handle_new_message_with_media(self, message: Message):
        telegram_file: File = self.__get_telegram_file(message)

        me = await self.telegram_client.get_me()
        filtered_users: list[User] = await self._get_users_without_me(message.chat, me.id)
        await self.chat_interactor.add_new_users(filtered_users, telegram_file.chat_telegram_id)

        file: BytesIO = await self._download_file(message)
        await self.loader_interactor.save_file(telegram_file, file)

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
            type=type_map[file_type],
            upload_date=message.date,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id,
        )

        return telegram_file

    # TODO: Перенести этот метод в новый класс UrlHandler, а этот переименовать в FileHandler
    async def _handle_new_message_with_urls(self, message: Message, urls: list[str]):
        for url in urls:
            name = self.get_url_name(url)
            file_info = File(
                name=name,
                extension='',
                type=FileTypeEnum.Link,
                upload_date=datetime.now(),
                sender_telegram_id=message.sender_id,
                chat_telegram_id=message.chat_id
            )
            file = BytesIO(bytes(url, encoding='utf-8'),)
            await self.loader_interactor.save_file(file_info, file)

    # TODO: Вынести в метод репозитория
    @staticmethod
    def get_url_name(url: str) -> str:
        try:
            html_data = urlopen(url)
            soup = BeautifulSoup(html_data, 'lxml')
            return soup.title.string
        except URLError:
            short_url = re.sub(r'(https?://)?(www\.)?', '', url)
            return short_url
