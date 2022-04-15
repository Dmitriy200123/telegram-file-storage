
import pytest
from clients.documents_index_client import DocumentsIndexClient


@pytest.fixture(scope='session')
def search_documents_client():
    client = DocumentsIndexClient()

    return client
