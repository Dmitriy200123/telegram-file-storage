from common.repository.base_repository import BaseRepository
from postgres.models.db_models import Chat


class ChatRepository(BaseRepository):

    async def is_contains_chat(self, chat_id: int) -> bool:
        return await self.adapter.contains(model=Chat, TelegramId=chat_id)

    async def find_chat_by_id(self, chat_id: int) -> Chat:
        return await self.adapter.get(model=Chat, TelegramId=chat_id)

    async def create_chat(self) -> Chat:
        raise NotImplementedError()
