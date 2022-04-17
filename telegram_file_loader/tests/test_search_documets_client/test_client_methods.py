from io import BytesIO

import pytest
from aiohttp.client_exceptions import ClientResponseError


async def test_search_document_success(search_documents_client, index_documents_client):
    document_id = 'faa49f08-60c0-42b2-83bc-fa205a39a604'
    await index_documents_client.index_document(
        document_id=document_id,
        name='Test document name',
        content=BytesIO(b'some test content'),
    )
    result = await search_documents_client.contains_in_name(document_id=document_id, queries=['test'])

    assert result == 'true'


async def test_search_cyrillic_name_document_success(search_documents_client, index_documents_client):
    document_id = 'faa49f08-60c0-42b2-83bc-fa205a39a604'
    await index_documents_client.index_document(
        document_id=document_id,
        name='Техзадание Егору.docx',
        content=BytesIO(b'some test content'),
    )
    result = await search_documents_client.contains_in_name(document_id=document_id, queries=['Егор'])

    assert result == 'true'


async def test_search_document_error(search_documents_client, index_documents_client):
    document_id = 'faa49f08-60c0-42b2-83bc-fa205a39a604'
    await index_documents_client.index_document(
        document_id=document_id,
        name='Test document name',
        content=BytesIO(b'some test content'),
    )
    result = await search_documents_client.contains_in_name(
        document_id=document_id,
        queries=['not_in_name']
    )

    assert result == 'false'


async def test_search_unknown_document_error(search_documents_client, index_documents_client):
    with pytest.raises(ClientResponseError):
        await search_documents_client.contains_in_name(
            document_id='document_id',
            queries=['nnn']
        )
