from telethon import TelegramClient
from telethon.events import ChatAction, Raw
from telethon.tl.types import UpdateNewChannelMessage

# TODO:
#  Добавлен в чат
#  Чат переименовали, изменили аватарку
#  Изменился Id чата (при миграции с группы на супергруппу)
#  Пользователя исключили из чата
#  Добавили нового участника в чат - сохранить инфу об этом участнике


class ChatHandler:
    telegram_client: TelegramClient

    def __init__(self, telegram_client: TelegramClient):
        self.telegram_client = telegram_client
        self.run()

    def run(self):
        chat_event = ChatAction()
        chat_migrate_event = Raw(types=UpdateNewChannelMessage)
        self.telegram_client.add_event_handler(
            self.__handle_chat_event, chat_event)
        self.telegram_client.add_event_handler(
            self.__handle_migrate_chat_event, chat_migrate_event)

    async def __handle_chat_event(self, event: ChatAction.Event):
        print()
        pass

    async def __handle_migrate_chat_event(self, event: UpdateNewChannelMessage):
        print()
        pass
