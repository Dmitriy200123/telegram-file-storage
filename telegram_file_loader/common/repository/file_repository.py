from io import BytesIO
from uuid import UUID, uuid4

from clients.s3_client import S3Client
from common.repository.base_repository import BaseRepository
from postgres.models.db_models import File
from postgres.models.external_models import File as FileExternal
from postgres.pg_adapter import Adapter


class FileRepository(BaseRepository):
    s3_client: S3Client

    def __init__(self, adapter: Adapter, s3_client: S3Client):
        super(FileRepository, self).__init__(adapter)
        self.s3_client = s3_client

    async def create_or_get(self, file_info: FileExternal, chat_id: UUID, file_sender_id: UUID) -> File:
        file_tuple: (File, bool) = await self.adapter.get_or_create(
            model=File,
            ChatId=chat_id,
            FileSenderId=file_sender_id,
            **file_info.dict_non_empty_fields()
        )

        return file_tuple[0]

    async def save_file(self, file: BytesIO, key=None) -> UUID:
        key: UUID = key or uuid4()
        await self.s3_client.upload_file(key=str(key), file=file)
        return key
