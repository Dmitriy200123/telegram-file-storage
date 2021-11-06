from typing import List

from telegram_file_loader.common.postgres.basic import manager
from telegram_file_loader.models import Chat, File, FileSender
from telegram_file_loader.tests.conftest import create_chat, create_file, create_sender


async def test_create_chat():
    await create_chat()
    result = await manager.execute(Chat.select())

    assert len(result) == 1


async def test_contains_chat():
    chat = await create_chat(chat_name='mysupername')
    await create_chat()
    found_chats: List[Chat] = await Chat.find_by_name('super')

    assert chat in found_chats


async def test_no_found_chats():
    await create_chat()
    found_chats = await Chat.find_by_name('doesnt exist i believes')
    assert len(found_chats) == 0


async def test_find_chat_by_telegram_id():
    await create_chat(telegram_id=123)
    await create_chat(telegram_id=228)
    chat = await create_chat(telegram_id=322)

    found_chats: List[Chat] = await Chat.find_by_telegram_id(322)

    assert chat in found_chats


async def test_update_chat_information():
    chat = await create_chat()
    chat_data_old = chat.as_dict()

    result = await Chat.get(Id=chat.Id)
    assert result.as_dict() == chat_data_old

    await chat.update_instance(Name='my new name here')

    result = await Chat.get(Id=chat.Id)
    new_chat_data = result.as_dict()
    assert new_chat_data != chat_data_old
    assert new_chat_data['Name'] == 'my new name here'


async def test_get_sender():
    sender = await create_sender()

    result = await manager.execute(FileSender.select())
    assert len(result) == 1
    assert sender in result


async def test_get_file():
    chat = await create_chat()
    sender = await create_sender()
    file = await create_file(sender=sender, chat=chat)

    result = list(await manager.execute(File.select()))

    assert len(result) == 1
    assert file in result
