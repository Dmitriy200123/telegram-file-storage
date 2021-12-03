from io import BytesIO

from common.interactor.base_interactor import BaseInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from postgres.models.db_models import File
from postgres.models.external_models import File as FileExternal


class LoaderInteractor(BaseInteractor):

    def __init__(
        self,
        chat_repository: ChatRepository,
        file_sender_repository: FileSenderRepository,
        file_repository: FileRepository
    ):
        super(LoaderInteractor, self).__init__(chat_repository)

        self.file_sender_repository = file_sender_repository
        self.file_repository = file_repository

    async def save_file(self, telegram_file: FileExternal, file: BytesIO):
        file_sender = await self.file_sender_repository.find_file_sender_by_id(telegram_file.sender_telegram_id)
        chat = await self.chat_repository.find_chat_by_telegram_id(telegram_file.chat_telegram_id)
        file_info: File = await self.file_repository.create_file_info(telegram_file, chat.Id, file_sender.Id)

        await self.file_repository.save_file(file_info.Name, file, file_info.Id)
