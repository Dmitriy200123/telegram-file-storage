import asyncio
import uuid

import pytest
import uvloop
from common import postgres
from common.utils import now_utc
from models import Chat, File, FileSender, FileTypeEnum


@pytest.fixture(scope='session', autouse=True)
def loop():
    uvloop.install()
    yield asyncio.get_event_loop()


@pytest.fixture(scope='session', autouse=True)
def init_db():
    postgres.start()
    Chat.create_table()
    FileSender.create_table()
    File.create_table()

    return postgres


@pytest.fixture(autouse=True)
def clean_db(init_db):
    yield
    Chat.truncate_table(cascade=True)
    File.truncate_table(cascade=True)
    FileSender.truncate_table(cascade=True)


async def create_chat(image_id=None, telegram_id=None, chat_name=None):
    chat = await Chat.create(
        Name=chat_name or 'test',
        ImageId=image_id or uuid.uuid4(),
        TelegramId=telegram_id or 123
    )
    return chat


async def create_sender(name=None, telegram_id=None, telegram_username=None):
    sender = await FileSender.create(
        TelegramId=telegram_id or 123,
        TelegramUserName=telegram_username or 'tele user name',
        FullName=name or 'test name',
    )
    return sender


async def create_file(
        sender: FileSender,
        chat: Chat,
        filename=None,
        extension=None,
        typeId=None,
        upload_date=None,
):
    file = await File.create(
        Name=filename or 'my filename',
        Extension=extension or 'file extension',
        TypeId=typeId or FileTypeEnum.Document,
        UploadDate=upload_date or now_utc(),
        FileSenderId=sender.Id,
        ChatId=chat.Id,
    )
    return file
