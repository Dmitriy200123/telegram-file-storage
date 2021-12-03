from postgres.models.db_models import FileSender
from postgres.models.external_models import FileSender as FileSenderExternal
from common.repository.base_repository import BaseRepository


class FileSenderRepository(BaseRepository):

    async def find_file_sender_by_id(self, sender_id: int) -> FileSender:
        return await self.adapter.get(model=FileSender, TelegramId=sender_id)

    async def update_file_sender(self, file_sender_external: FileSenderExternal):
        file_sender: FileSender = await self.find_file_sender_by_id(file_sender_external.telegram_id)
        await self.adapter.update(model=file_sender, **file_sender_external.dict(by_alias=True))

    async def create_file_sender(self, file_sender_external: FileSenderExternal) -> FileSender:
        if not await self.adapter.contains(model=FileSender, TelegramId=file_sender_external.telegram_id):
            return await self.adapter.create(model=FileSender, **file_sender_external.dict(by_alias=True))
        else:
            return await self.adapter.get(model=FileSender, TelegramId=file_sender_external.telegram_id)
