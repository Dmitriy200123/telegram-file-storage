import pytest
from clients.s3_client import S3Client


@pytest.fixture
def s3_client():
    client = S3Client('test')

    return client


@pytest.fixture
async def init_s3(s3_client):
    await s3_client.create_or_get_bucket('test')


@pytest.fixture
async def clean_s3(s3_client):
    yield
    files = await s3_client.get_objects()
    await files.delete()
