import logging
from io import BytesIO

import aioboto3
import config
from aiohttp import StreamReader
from botocore.exceptions import ClientError


class S3Client(aioboto3.session.Session):
    """
    https://aioboto3.readthedocs.io/en/latest/usage.html
    """

    session: aioboto3.Session
    aws_access_key_id: str
    aws_secret_access_key: str
    bucket_name: str
    url: str

    def __init__(self, bucket_name: str, url=None, aws_access_key_id=None, aws_secret_access_key=None, **kwargs):
        self.url = url or config.S3_URL
        self.aws_access_key_id = aws_access_key_id or config.AWS_ACCESS_KEY_ID
        self.aws_secret_access_key = aws_secret_access_key or config.AWS_SECRET_ACCESS_KEY
        self.bucket_name = bucket_name
        self.session = aioboto3.Session()
        super().__init__(**kwargs)

    async def upload_file(self, file: BytesIO, key=None) -> bool:
        """Загружает файлик в с3

        :param file: read-like байты файла
        :param key: имя файла в с3. Если не указано берет путь файла
        :return: Удачно или нет загружен
        """

        async with self.client(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as client:
            try:
                await client.upload_fileobj(file, self.bucket_name, key)
            except ClientError as e:
                logging.error(e)
                return False
        return True

    async def download_file(self, key) -> StreamReader:
        """Загружает файлик из s3  в aiohttp стрим

        :param key: имя файла в с3
        :return: Стрим с файлом
        """
        async with self.client(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as client:
            try:
                s3_ob = await client.get_object(Bucket=self.bucket_name, Key=key)
                return s3_ob['Body']
            except ClientError:
                raise FileNotFoundError(
                    f'No file with {key=} in bucket {self.bucket_name}')

    async def get_objects(self):
        """Возвращает все объекты из корзины

        :return: объекты из Bucket
        """
        async with self.resource(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as s3:
            bucket = await s3.Bucket(self.bucket_name)
            return await bucket.objects.all()
