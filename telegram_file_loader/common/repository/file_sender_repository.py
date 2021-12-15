from typing import Any

from common.repository.base_repository import BaseRepository
from postgres.models.db_models import FileSender
from postgres.models.external_models import FileSender as FileSenderExternal


class FileSenderRepository(BaseRepository):

    async def find_by_telegram_id(self, telegram_id: int) -> Any:
        return await self.adapter.get(model=FileSender, TelegramId=telegram_id)

    async def update_or_create(self, file_sender_external: FileSenderExternal) -> FileSender:
        file_sender, is_created = await self.adapter.create_or_get(
            model=FileSender,
            **file_sender_external.dict_non_empty_fields()
        )

        if not is_created:
            await self.adapter.update(model=file_sender, **file_sender_external.dict_non_empty_fields())

        return file_sender

    async def create_or_get(self, file_sender_external: FileSenderExternal) -> FileSender:
        file_sender_tuple: (FileSender, bool) = await self.adapter.create_or_get(
            model=FileSender,
            **file_sender_external.dict_non_empty_fields()
        )
        return file_sender_tuple[0]

    async def contains_by_telegram_id(self, telegram_id: int) -> bool:
        return await self.adapter.contains(model=FileSender, TelegramId=telegram_id)
