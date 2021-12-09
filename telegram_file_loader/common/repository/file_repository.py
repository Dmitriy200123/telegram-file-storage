from io import BytesIO

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

    async def create_file_info(self, file_info: FileExternal) -> File:
        return await self.adapter.create(model=File, **file_info.dict(by_alias=True))

    async def save_file(self, file_info: File, file: BytesIO):
        await self.s3_client.upload_file(key=str(file_info.Id), file=file)
        # await self.s3_client.upload_file(key=str(file_info.Id), filename=file_info.Name, file=file)

        # print(await self.s3_client.get_download_link(str(file_info.Id)))
