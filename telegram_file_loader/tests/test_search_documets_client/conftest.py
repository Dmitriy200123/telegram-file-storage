
import pytest
from clients.search_documents_api_client import SearchDocumentsClient


@pytest.fixture(scope='session')
def search_documents_client():
    client = SearchDocumentsClient()

    return client
