from io import BytesIO

import pytest


async def test_s3_upload_file(s3_client):
    file = b'\x01' * 128
    response = await s3_client.upload_file(BytesIO(file), 'test_file')

    assert response


async def test_s3_get_file(s3_client):
    file = b'\x01' * 128
    await s3_client.upload_file(BytesIO(file), 'test_file')
    result = await s3_client.download_file('test_file')
    res = await result.read()

    assert res == file


async def test_s3_get_unknown_file(s3_client):
    with pytest.raises(FileNotFoundError):
        await s3_client.download_file('very_rare_file')

    assert True
