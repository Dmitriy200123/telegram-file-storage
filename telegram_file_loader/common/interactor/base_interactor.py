from typing import Any

from common.repository.chat_repository import ChatRepository
from common.repository.tag_repository import TagRepository
from common.repository.url_repository import UrlRepository


class BaseInteractor:

    def __init__(
        self,
        chat_repository: ChatRepository,
        url_repository: UrlRepository,
        tag_repository: TagRepository
    ):
        self.chat_repository = chat_repository
        self.url_repository = url_repository
        self.tag_repository = tag_repository

    async def is_valid_chat(self, chat_id: int) -> bool:
        return await self.chat_repository.contains_by_telegram_id(chat_id)

    def find_urls(self, message: str) -> Any:
        if not self.url_repository.has_urls(message):
            return None

        return self.url_repository.find_urls(message)

    def get_url_name(self, url: str) -> str:
        return self.url_repository.get_name(url)

    async def find_marked_text(self, message: str) -> Any:
        if not await self.tag_repository.has_tags(message):
            return None

        return await self.tag_repository.find_marked_text(message)
