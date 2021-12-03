from uuid import UUID

from common.repository.base_repository import BaseRepository
from postgres.models.db_models import SenderToChat


class SenderToChatRepository(BaseRepository):

    async def create_sender_to_chat(self, sender_id: UUID, chat_id: UUID) -> SenderToChat:
        if not await self.adapter.contains(model=SenderToChat, SenderId=sender_id, ChatId=chat_id):
            return await self.adapter.create(model=SenderToChat, SenderId=sender_id, ChatId=chat_id)
        else:
            return await self.adapter.get(model=SenderToChat, SenderId=sender_id, ChatId=chat_id)

    async def find_sender_to_chat(self, sender_id: UUID, chat_id: UUID) -> SenderToChat:
        return await self.adapter.get(model=SenderToChat, SenderId=sender_id, ChatId=chat_id)

    @staticmethod
    async def delete_sender_to_chat(sender_to_chat: SenderToChat):
        sender_to_chat.delete_instance()
