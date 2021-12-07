from uuid import UUID

from common.repository.base_repository import BaseRepository
from postgres.models.db_models import Chat
from postgres.models.external_models import Chat as ChatExternal


class ChatRepository(BaseRepository):

    async def is_contains_chat(self, chat_id: int) -> bool:
        return await self.adapter.contains(model=Chat, TelegramId=chat_id)

    async def find_chat_by_telegram_id(self, telegram_id: int) -> Chat:
        return await self.adapter.get(model=Chat, TelegramId=telegram_id)

    async def create_chat(self, chat_external: ChatExternal, chat_photo_id: UUID) -> Chat:
        if not await self.adapter.contains(model=Chat, TelegramId=chat_external.telegram_id):
            return await self.adapter.create(model=Chat, ImageId=chat_photo_id, **chat_external.dict(by_alias=True))
        else:
            return await self.adapter.get(model=Chat, TelegramId=chat_external.telegram_id)

    async def update_chat(self, updated_chat: Chat):
        chat = await self.adapter.get(model=Chat, Id=updated_chat.Id)
        await self.adapter.update(chat, **updated_chat.as_dict())
