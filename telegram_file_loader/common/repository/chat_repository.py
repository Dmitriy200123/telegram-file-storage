from common.repository.base_repository import BaseRepository
from peewee import DoesNotExist
from postgres.models.db_models import Chat
from postgres.models.external_models import Chat as ChatExternal


class ChatRepository(BaseRepository):

    async def contains_by_telegram_id(self, telegram_id: int) -> bool:
        return await self.adapter.contains(model=Chat, TelegramId=telegram_id)

    async def find_by_telegram_id(self, telegram_id: int) -> Chat:
        return await self.adapter.get(model=Chat, TelegramId=telegram_id)

    async def create_or_get(self, chat_external: ChatExternal) -> Chat:
        chat_tuple: (Chat, bool) = await self.adapter.create_or_get(
            model=Chat,
            **chat_external.dict(by_alias=True)
        )

        return chat_tuple[0]

    async def update(self, updated_chat: Chat):
        chat: Chat = await self.adapter.get(model=Chat, Id=updated_chat.Id)

        if chat is None:
            raise DoesNotExist('Обновляемого чата не существует')

        await self.adapter.update(chat, **updated_chat.as_dict())
