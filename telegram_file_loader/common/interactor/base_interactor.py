from typing import Any

from common.repository.chat_repository import ChatRepository
from common.repository.url_repository import UrlRepository


class BaseInteractor:

    def __init__(self, chat_repository: ChatRepository, url_repository: UrlRepository):
        self.chat_repository = chat_repository
        self.url_repository = url_repository

    async def is_valid_chat(self, chat_id: int) -> bool:
        return await self.chat_repository.contains_by_telegram_id(chat_id)

    def find_urls(self, message: str) -> Any:
        if not self.url_repository.has_urls(message):
            return None

        return self.url_repository.find_urls(message)

    def get_url_name(self, url: str) -> str:
        return self.url_repository.get_name(url)
