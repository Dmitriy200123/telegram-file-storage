from postgres.models.db_models import FileSender
from common.repository.base_repository import BaseRepository


class FileSenderRepository(BaseRepository):

    async def find_file_sender_by_id(self, sender_id: int) -> FileSender:
        return await self.adapter.get(model=FileSender, TelegramId=sender_id)
