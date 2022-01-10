from io import BytesIO

from common.interactor.base_interactor import BaseInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from common.repository.sender_to_chat_repository import SenderToChatRepository
from common.repository.tag_repository import TagRepository
from common.repository.url_repository import UrlRepository
from common.utils import full_name
from postgres.models.db_models import Chat, FileSender
from postgres.models.external_models import Chat as ChatExternal
from postgres.models.external_models import FileSender as FileSenderExternal
from telethon.tl.types import User


class ChatInteractor(BaseInteractor):

    def __init__(
        self,
        chat_repository: ChatRepository,
        file_sender_repository: FileSenderRepository,
        file_repository: FileRepository,
        sender_to_chat_repository: SenderToChatRepository,
        url_repository: UrlRepository,
        tag_repository: TagRepository
    ):
        super(ChatInteractor, self).__init__(
            chat_repository, url_repository, tag_repository)

        self.file_sender_repository = file_sender_repository
        self.file_repository = file_repository
        self.sender_to_chat_repository = sender_to_chat_repository

    async def update_file_sender(self, telegram_sender: FileSenderExternal, chat_id: int):
        sender: FileSender = await self.file_sender_repository.update_or_create(telegram_sender)
        chat: Chat = await self.chat_repository.find_by_telegram_id(chat_id)

        await self.sender_to_chat_repository.create_or_get(sender.Id, chat.Id)

    async def migrate_chat(self, old_telegram_id: int, new_telegram_id: int):
        chat = await self.chat_repository.find_by_telegram_id(old_telegram_id)
        chat.TelegramId = new_telegram_id

        await self.chat_repository.update(chat)

    async def add_chat(self, chat: ChatExternal, chat_photo: BytesIO, users: list[User]):
        photo_key = None

        if chat_photo is not None:
            photo_key = await self.file_repository.save_file(chat_photo)

        chat.image_id = photo_key
        chat_db = await self.chat_repository.create_or_get(chat)

        await self.add_new_users(users, chat_db.TelegramId)

    async def add_new_users(self, users: list[User], chat_id: int):
        chat = await self.chat_repository.find_by_telegram_id(chat_id)

        for user in users:
            file_sender_external = FileSenderExternal(
                telegram_id=user.id,
                telegram_username=user.username,
                full_name=full_name(user)
            )
            file_sender = await self.file_sender_repository.create_or_get(file_sender_external)

            await self.sender_to_chat_repository.create_or_get(file_sender.Id, chat.Id)

    async def delete_user_from_chat(self, user_id: int, chat_id: int):
        sender = await self.file_sender_repository.find_by_telegram_id(user_id)
        chat = await self.chat_repository.find_by_telegram_id(chat_id)

        sender_to_chat = await self.sender_to_chat_repository.find_by_sender_and_chat_ids(sender.Id, chat.Id)
        await self.sender_to_chat_repository.delete(sender_to_chat)

    async def update_chat_name(self, new_chat_name: str, chat_id: int):
        chat = await self.chat_repository.find_by_telegram_id(chat_id)
        chat.Name = new_chat_name
        await self.chat_repository.update(chat)

    async def update_chat_photo(self, photo: BytesIO, chat_id: int):
        image_key = await self.file_repository.save_file(photo)

        chat = await self.chat_repository.find_by_telegram_id(chat_id)
        chat.ImageId = image_key

        await self.chat_repository.update(chat)
