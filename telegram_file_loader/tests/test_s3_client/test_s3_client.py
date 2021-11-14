from io import BytesIO

import config
import pytest


async def test_s3_upload_file(s3_client):
    file = b'\x01' * 128
    response = await s3_client.upload_file(file=BytesIO(file), key='test_file')

    assert response


async def test_s3_get_file(s3_client):
    file = b'\x01' * 128
    await s3_client.upload_file(file=BytesIO(file), key='test_file')
    result = await s3_client.download_file('test_file')
    res = await result.read()

    assert res == file


async def test_s3_get_unknown_file(s3_client):
    with pytest.raises(FileNotFoundError):
        await s3_client.download_file('very_rare_file')

    assert True


async def tests_s3_get_download_link(s3_client):
    file = b'\x01' * 128
    file_name = 'test_file_name'
    await s3_client.upload_file(file=BytesIO(file), key=file_name)
    url = await s3_client.get_download_link(file_name)

    assert f'{config.S3_URL}/test/{file_name}?AWSAccessKeyId={config.AWS_ACCESS_KEY_ID}' in url
