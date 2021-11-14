import asyncio

import postgres
import pytest
from pg_adapter import Adapter
from postgres.models.db_models import Chat, File, FileSender


@pytest.fixture(scope='session', autouse=True)
def loop():
    loop = asyncio.get_event_loop()
    yield loop
    loop.close()


@pytest.fixture(scope='session', autouse=True)
def init_db():
    postgres.start()
    Chat.create_table()
    FileSender.create_table()
    File.create_table()

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
