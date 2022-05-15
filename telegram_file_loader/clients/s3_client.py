import logging
from io import BytesIO

import config
from aiobotocore import get_session
from aiohttp import StreamReader
from botocore.exceptions import ClientError


class S3Client:
    """
    https://aiobotocore.readthedocs.io/en/latest/
    """

    aws_access_key_id: str
    aws_secret_access_key: str
    bucket_name: str
    url: str

    def __init__(self, bucket_name: str, url=None, aws_access_key_id=None, aws_secret_access_key=None, **kwargs):
        self.url = url or config.S3_URL
        self.aws_access_key_id = aws_access_key_id or config.AWS_ACCESS_KEY_ID
        self.aws_secret_access_key = aws_secret_access_key or config.AWS_SECRET_ACCESS_KEY
        self.bucket_name = bucket_name
        super().__init__(**kwargs)

    async def create_or_get_bucket(self, bucket_name=None):
        """Создает корзину, если такая такой нет, и возвращает ее

        :param bucket_name: имя корзины

        :return: Bucket из S3
        """

        session = get_session()
        async with session.resource(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as s3:
            return await s3.create_bucket(Bucket=bucket_name or self.bucket_name)

    async def upload_file(self, key: str, file: BytesIO, mime_type=None) -> bool:
        """Загружает файлик в S3

        :param file: read-like байты файла
        :param key: имя файла в S3
        :param mime_type: mime type файла
        :return: Удачно или нет загружен
        """

        session = get_session()
        async with session.create_client(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as client:
            try:
                await client.put_object(Body=file, Bucket=self.bucket_name, Key=key, ContentType=mime_type)
            except ClientError as e:
                logging.error(e)
                return False
        return True

    async def download_file(self, key: str) -> StreamReader:
        """Загружает файлик из S3  в aiohttp стрим

        :param key: имя файла в S3
        :return: Стрим с файлом
        """

        session = get_session()
        async with session.create_client(
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

    async def get_files(self):
        """Возвращает все объекты из корзины

        :return: объекты из Bucket
        """

        session = get_session()
        async with session.resource(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as s3:
            bucket = await s3.Bucket(self.bucket_name)
            return await bucket.objects.all()

    async def get_download_link(self, key: str, expires=config.S3_URL_EXPIRES_IN_SECONDS) -> str:
        """Возвращает урл для загрузки файла

        :param key: имя файла в S3
        :param expires: Время жизни ссылки в секундах
        :return: объекты из Bucket
        """

        session = get_session()
        async with session.create_client(
                service_name='s3',
                endpoint_url=self.url,
                aws_access_key_id=self.aws_access_key_id,
                aws_secret_access_key=self.aws_secret_access_key
        ) as client:
            try:
                await client.get_object(Bucket=self.bucket_name, Key=key)
                return await client.generate_presigned_url(
                    'get_object',
                    Params={'Bucket': self.bucket_name, 'Key': key},
                    ExpiresIn=expires
                )
            except ClientError:
                raise FileNotFoundError(
                    f'No file with {key=} in bucket {self.bucket_name}')
