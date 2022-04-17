import asyncio
import sys

import postgres
import pytest
from clients.documents_index_client import DocumentsIndexClient
from clients.documents_search_client import DocumentsSearchClient
from postgres.models.db_models import Chat, Code, File, FileSender, SenderToChat
from postgres.models.db_models.marked_text_tags import MarkedTextTags
from postgres.pg_adapter import Adapter


@pytest.fixture(scope='session', autouse=True)
def loop():
    if sys.platform == 'win32':
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())

    loop = asyncio.get_event_loop()
    yield loop
    loop.close()


@pytest.fixture(scope='session', autouse=True)
def init_db():
    postgres.start()
    Chat.create_table()
    FileSender.create_table()
    File.create_table()
    SenderToChat.create_table()
    MarkedTextTags.create_table()
    Code.create_table()

    return postgres.basic.manager


@pytest.fixture
def db_manager(init_db):
    adapter = Adapter(init_db)

    return adapter


@pytest.fixture(autouse=True)
def clean_db(init_db):
    yield
    Chat.truncate_table(cascade=True)
    File.truncate_table(cascade=True)
    FileSender.truncate_table(cascade=True)
    SenderToChat.truncate_table(cascade=True)
    MarkedTextTags.truncate_table(cascade=True)
    Code.truncate_table(cascade=True)


@pytest.fixture(scope='session')
def search_documents_client():
    client = DocumentsSearchClient()

    return client


@pytest.fixture(scope='session')
def index_documents_client():
    client = DocumentsIndexClient()

    return client
