from io import BytesIO

import pytest
from aiohttp.client_exceptions import ClientResponseError


async def test_index_document_success(search_documents_client):
    result = await search_documents_client.index_document(
        document_id='faa49f08-60c0-42b2-83bc-fa205a39a604',
        name='Test document name',
        content=BytesIO(b'some test content'),
    )

    assert result == ''


async def test_index_document_error(search_documents_client):
    with pytest.raises(ClientResponseError):
        await search_documents_client.index_document(
            document_id='wrong',
            name='Test document name',
            content=BytesIO(b''),
        )
