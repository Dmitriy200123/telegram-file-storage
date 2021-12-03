from common.repository.chat_repository import ChatRepository


class BaseInteractor:

    def __init__(self, chat_repository: ChatRepository):
        self.chat_repository = chat_repository

    async def is_valid_chat(self, chat_id: int) -> bool:
        return await self.chat_repository.is_contains_chat(chat_id)
