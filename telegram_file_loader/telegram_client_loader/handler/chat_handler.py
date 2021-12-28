from common import utils
from common.interactor.chat_interactor import ChatInteractor
from postgres.models.external_models import Chat, FileSender
from telegram_client_loader.handler.base_handler import BaseHandler
from telethon import TelegramClient
from telethon.errors import ChannelPrivateError
from telethon.events import ChatAction, Raw
from telethon.tl.patched import Message, MessageService
from telethon.tl.types import Chat as TelegramChat
from telethon.tl.types import (
    ChatPhotoEmpty,
    MessageActionChatAddUser,
    MessageActionChatCreate,
    MessageActionChatDeleteUser,
    MessageActionChatEditPhoto,
    MessageActionChatEditTitle,
    MessageActionChatJoinedByLink,
    MessageActionChatMigrateTo,
    PeerChannel,
    UpdateChannel,
    UpdateNewMessage,
    User,
)
from telethon.utils import get_peer_id


class ChatHandler(BaseHandler):

    def __init__(self, telegram_client: TelegramClient, chat_interactor: ChatInteractor):
        super(ChatHandler, self).__init__(telegram_client, chat_interactor)

        self.chat_interactor = chat_interactor
        self.run()

    def run(self):
        super(ChatHandler, self).run()

        chat_action_event = ChatAction()
        chat_migrate_event = Raw(types=[UpdateNewMessage])
        chat_client_joined_event = Raw(types=[UpdateChannel])

        self.telegram_client.add_event_handler(
            self.__handle_chat_event, chat_action_event)
        self.telegram_client.add_event_handler(
            self.__handle_migrate_chat_event, chat_migrate_event)
        self.telegram_client.add_event_handler(
            self.__handle_joined_client_event,
            chat_client_joined_event
        )

    async def _handle_new_message_with_media(self, message: Message):
        if isinstance(message.sender, User):
            telegram_sender = self.__get_telegram_sender(message.sender)
            await self.chat_interactor.update_file_sender(telegram_sender, message.chat_id)

    async def __handle_migrate_chat_event(self, event: UpdateNewMessage):
        message: MessageService = event.message

        if isinstance(message.action, MessageActionChatMigrateTo):
            old_id = message.chat_id
            new_id = get_peer_id(PeerChannel(message.action.channel_id))
            await self.chat_interactor.migrate_chat(old_id, new_id)

    @staticmethod
    def __get_telegram_sender(user: User) -> FileSender:
        return FileSender(telegram_id=user.id, telegram_username=user.username, full_name=utils.full_name(user))

    async def __handle_chat_event(self, event: ChatAction.Event):
        message: MessageService = event.action_message
        action = message.action
        telegram_chat: TelegramChat = event.chat

        if isinstance(action, MessageActionChatCreate) or isinstance(action, MessageActionChatAddUser):
            await self.__handle_add_users_to_chat(action, telegram_chat, event.chat_id)

        if isinstance(action, MessageActionChatJoinedByLink):
            await self.__handle_joined_user(message.sender_id, telegram_chat, event.chat_id)

        if isinstance(action, MessageActionChatDeleteUser):
            await self.__handle_delete_user(action, event.chat_id)

        if isinstance(action, MessageActionChatEditTitle):
            await self.__handle_edit_chat_title(action, event.chat_id)

        if isinstance(action, MessageActionChatEditPhoto):
            await self.__handle_edit_chat_photo(telegram_chat.id, event.chat_id)

    async def __handle_add_users_to_chat(
        self,
        action: MessageActionChatAddUser,
        telegram_chat: TelegramChat,
        chat_id: int
    ):
        me = await self.telegram_client.get_me()

        if me.id in action.users:
            await self.__add_new_chat(telegram_chat, chat_id, me.id)
        else:
            new_users = await self.__get_users_by_telegram_ids(action.users)
            await self.chat_interactor.add_new_users(new_users, chat_id)

    async def __handle_joined_user(
        self,
        sender_id: int,
        telegram_chat: TelegramChat,
        chat_id: int
    ):
        me = await self.telegram_client.get_me()
        user = await self.telegram_client.get_entity(sender_id)

        if me.id == user.id:
            await self.__add_new_chat(telegram_chat, chat_id, me.id)
        else:
            await self.chat_interactor.add_new_users([user], chat_id)

    async def __handle_joined_client_event(self, event: UpdateChannel):
        try:
            chat = await self.telegram_client.get_entity(event.channel_id)
            chat_id = get_peer_id(PeerChannel(event.channel_id))
            me = await self.telegram_client.get_me()

            await self.__add_new_chat(chat, chat_id, me.id)
        except ChannelPrivateError:
            pass

    async def __add_new_chat(self, telegram_chat: TelegramChat, chat_id: int, me_id: int):
        filtered_users: list[User] = await self._get_users_without_me(telegram_chat, me_id)

        chat = Chat(name=telegram_chat.title, telegram_id=chat_id)
        chat_photo = None

        if not isinstance(telegram_chat.photo, ChatPhotoEmpty):
            chat_photo = await self._download_chat_photo(telegram_chat.id)

        await self.chat_interactor.add_chat(chat, chat_photo, filtered_users)

    async def __handle_delete_user(self, action: MessageActionChatDeleteUser, chat_id: int):
        me = await self.telegram_client.get_me()

        if me.id != action.user_id:
            user = await self.telegram_client.get_entity(action.user_id)
            await self.chat_interactor.add_new_users([user], chat_id)
            await self.chat_interactor.delete_user_from_chat(action.user_id, chat_id)

    async def __handle_edit_chat_title(self, action: MessageActionChatEditTitle, chat_id: int):
        await self.chat_interactor.update_chat_name(action.title, chat_id)

    async def __handle_edit_chat_photo(self, channel_id: int, chat_id: int):
        photo = await self._download_chat_photo(channel_id)
        await self.chat_interactor.update_chat_photo(photo, chat_id)

    async def __get_users_by_telegram_ids(self, users_ids: list[int]) -> list[User]:
        users: list[User] = list()

        for user_id in users_ids:
            users.append(await self.telegram_client.get_entity(user_id))

        return users
