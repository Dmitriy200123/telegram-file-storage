from common.repository.base_repository import BaseRepository
from postgres.models.db_models import FileSender
from postgres.models.external_models import FileSender as FileSenderExternal


class FileSenderRepository(BaseRepository):

    async def find_file_sender_by_id(self, sender_id: int) -> FileSender:
        if not await self.adapter.contains(model=FileSender, TelegramId=sender_id):
            return None

        return await self.adapter.get(model=FileSender, TelegramId=sender_id)

    async def update_file_sender(self, file_sender_external: FileSenderExternal):
        file_sender: FileSender = await self.find_file_sender_by_id(file_sender_external.telegram_id)

        if file_sender is None:
            return await self.create_file_sender(file_sender_external)
        else:
            await self.adapter.update(model=file_sender, **file_sender_external.dict(by_alias=True))

    async def create_file_sender(self, file_sender_external: FileSenderExternal) -> FileSender:
        file_sender_tuple: (FileSender, bool) = await self.adapter.create_or_get(
            model=FileSender,
            **file_sender_external.dict(by_alias=True)
        )
        return file_sender_tuple[0]

    async def contains_by_telegram_id(self, telegram_id: int) -> bool:
        return await self.adapter.contains(model=FileSender, TelegramId=telegram_id)
