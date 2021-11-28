from io import BytesIO
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from postgres.models.db_models import File
from telegram_client_loader.model.telegram_file import TelegramFile


class LoaderInteractor:
    chat_repository: ChatRepository
    file_sender_repository: FileSenderRepository
    file_repository: FileRepository

    def __init__(
        self,
        chat_repository: ChatRepository,
        file_sender_repository: FileSenderRepository,
        file_repository: FileRepository
    ):
        self.chat_repository = chat_repository
        self.file_sender_repository = file_sender_repository
        self.file_repository = file_repository

    async def is_valid_chat(self, chat_id: int) -> bool:
        return await self.chat_repository.is_contains_chat(chat_id)

    # TODO: Обновлять запись о отправителе, если изменилось имя или юзернейм
    async def update_file_sender(self):
        pass

    async def save_file(self, telegram_file: TelegramFile, file: BytesIO):
        file_sender = await self.file_sender_repository.find_file_sender_by_id(telegram_file.sender_id)
        chat = await self.chat_repository.find_chat_by_id(telegram_file.chat_id)

        file_info_external = telegram_file.to_file(chat.Id, file_sender.Id)
        file_info: File = await self.file_repository.create_file_info(file_info_external)

        await self.file_repository.save_file(file_info, file)
