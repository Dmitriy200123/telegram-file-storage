from telethon import TelegramClient
from telethon.events import ChatAction, Raw, NewMessage
from telethon.tl.patched import MessageService, Message
from telethon.tl.types import Chat as TelegramChat, ChatPhotoEmpty
from telethon.tl.types import User
from common import utils
from common.interactor.chat_interactor import ChatInteractor
from postgres.models.external_models import FileSender, Chat
from telegram_client_loader.handler.base_handler import BaseHandler
from telethon.tl.types import (UpdateNewMessage, MessageActionChatMigrateTo, MessageActionChatCreate,
                               MessageActionChatAddUser, MessageActionChatDeleteUser, MessageActionChatEditTitle,
                               MessageActionChatEditPhoto)

class ChatHandler(BaseHandler):

    def __init__(self, telegram_client: TelegramClient, chat_interactor: ChatInteractor):
        super(ChatHandler, self).__init__(telegram_client, chat_interactor)

        self.chat_interactor = chat_interactor
        self.run()

    def run(self):
        new_message_event = NewMessage()
        chat_event = ChatAction()
        chat_migrate_event = Raw(types=[UpdateNewMessage])

        self.telegram_client.add_event_handler(self.__handle_change_sender_info_event, new_message_event)
        self.telegram_client.add_event_handler(
            self.__handle_chat_event, chat_event)
        self.telegram_client.add_event_handler(
            self.__handle_migrate_chat_event, chat_migrate_event)

    async def __handle_migrate_chat_event(self, event: UpdateNewMessage):
        message: MessageService = event.message
        # is_valid = await self.is_valid_chat(message.chat_id)

        # if is_valid and isinstance(message.action, MessageActionChatMigrateTo):
        if isinstance(message.action, MessageActionChatMigrateTo):
            old_id = message.chat_id
            new_id = int(f'-100{message.action.channel_id}')
            await self.chat_interactor.migrate_chat(old_id, new_id)

    async def __handle_change_sender_info_event(self, event: NewMessage.Event):
        message: Message = event.message
        # is_valid = await self.is_valid_chat(message.chat_id)

        # if is_valid and message.media and isinstance(message.sender, User):
        if message.media and isinstance(message.sender, User):
            telegram_sender = self.__get_telegram_sender(message.sender)
            await self.chat_interactor.update_file_sender(telegram_sender)

    @staticmethod
    def __get_telegram_sender(user: User) -> FileSender:
        return FileSender(telegram_id=user.id, telegram_username=user.username, full_name=utils.full_name(user))

    async def __handle_chat_event(self, event: ChatAction.Event):
        # is_valid = await self.is_valid_chat(event.chat_id)
        action = event.action_message.action
        telegram_chat: TelegramChat = event.chat
        telegram_chat.id = event.chat_id

        if isinstance(action, MessageActionChatCreate) or isinstance(action, MessageActionChatAddUser):
            await self.__handle_add_users_to_chat(action, telegram_chat)

        if isinstance(action, MessageActionChatDeleteUser):
            await self.__handle_delete_user(action, telegram_chat.id)

        if isinstance(action, MessageActionChatEditTitle):
            await self.__handle_edit_chat_title(action, telegram_chat.id)

        if isinstance(action, MessageActionChatEditPhoto):
            await self.__handle_edit_chat_photo(telegram_chat)

    async def __handle_add_users_to_chat(self, action: MessageActionChatAddUser, telegram_chat: TelegramChat):
        me = await self.telegram_client.get_me()

        if me.id in action.users:
            users: list[User] = await self.telegram_client.get_participants(telegram_chat)
            filtered_users: list[User] = list(filter(lambda user: user.id != me.id, users))

            chat = Chat(name=telegram_chat.title, telegram_id=telegram_chat.id)
            chat_photo = None

            if not isinstance(telegram_chat.photo, ChatPhotoEmpty):
                chat_photo = await self.download_chat_photo(telegram_chat)

            await self.chat_interactor.add_chat(chat, chat_photo, filtered_users)
        else:
            new_users = await self.__get_users_by_telegram_ids(action.users)
            await self.chat_interactor.add_new_users(new_users, telegram_chat.id)

    async def __handle_delete_user(self, action: MessageActionChatDeleteUser, chat_id: int):
        me = await self.telegram_client.get_me()

        if me.id != action.user_id:
            await self.chat_interactor.delete_user_from_chat(action.user_id, chat_id)

    async def __handle_edit_chat_title(self, action: MessageActionChatEditTitle, chat_id: int):
        await self.chat_interactor.update_chat_name(action.title, chat_id)

    async def __handle_edit_chat_photo(self, telegram_chat: TelegramChat):
        photo = await self.download_chat_photo(telegram_chat)
        await self.chat_interactor.update_chat_photo(photo, telegram_chat.id)

    async def __get_users_by_telegram_ids(self, users_ids: list[int]) -> list[User]:
        users: list[User] = list()

        for user_id in users_ids:
            users.append(await self.telegram_client.get_entity(user_id))

        return users
