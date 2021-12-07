import typing
from io import BytesIO

from common.interactor.base_interactor import BaseInteractor
from telethon import TelegramClient
from telethon.tl.custom import Message


class BaseHandler:

    def __init__(
        self,
        telegram_client: TelegramClient,
        base_interactor: BaseInteractor
    ):
        self.telegram_client = telegram_client
        self.loader_interactor = base_interactor

    async def is_valid_chat(self, chat_id: int) -> bool:
        return await self.loader_interactor.is_valid_chat(chat_id)

    async def download_file(self, message: Message) -> BytesIO:
        file: BytesIO = typing.cast(BytesIO, await self.telegram_client.download_media(message, file=BytesIO()))
        return BytesIO(file.getvalue())

    async def download_chat_photo(self, channel_id: int) -> BytesIO:
        photo_bytes = typing.cast(
            BytesIO,
            await self.telegram_client.download_profile_photo(entity=channel_id, file=BytesIO())
        )
        return BytesIO(photo_bytes.getvalue())
