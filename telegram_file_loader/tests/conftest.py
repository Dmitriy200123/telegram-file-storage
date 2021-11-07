import asyncio

import pytest
from common import db
from models.db_models import Chat, File, FileSender


@pytest.fixture(scope='session', autouse=True)
def loop():
    loop = asyncio.get_event_loop()
    yield loop
    loop.close()


@pytest.fixture(scope='session', autouse=True)
def init_db():
    db.start()
    Chat.create_table()
    FileSender.create_table()
    File.create_table()

    return db


@pytest.fixture(autouse=True)
def clean_db(init_db):
    yield
    Chat.truncate_table(cascade=True)
    File.truncate_table(cascade=True)
    FileSender.truncate_table(cascade=True)
