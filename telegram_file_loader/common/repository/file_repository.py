from io import BytesIO
from uuid import UUID, uuid4
from clients.s3_client import S3Client
from postgres.models.db_models import File
from postgres.models.external_models import File as FileExternal
from postgres.pg_adapter import Adapter
from common.repository.base_repository import BaseRepository


class FileRepository(BaseRepository):
    s3_client: S3Client

    def __init__(self, adapter: Adapter, s3_client: S3Client):
        super(FileRepository, self).__init__(adapter)
        self.s3_client = s3_client

    async def create_file_info(self, file_info: FileExternal, chat_id: UUID, file_sender_id: UUID) -> File:
        return await self.adapter.create(
            model=File,
            ChatId=chat_id,
            FileSenderId=file_sender_id,
            **file_info.dict(by_alias=True)
        )

    async def save_file(self, filename: str, file: BytesIO, key=None) -> UUID:
        key = key or uuid4()
        await self.s3_client.upload_file(key=str(key), file=file)
        # await self.s3_client.upload_file(key=str(key), filename=filename, file=file)

        # print(await self.s3_client.get_download_link(str(key)))

        return key
