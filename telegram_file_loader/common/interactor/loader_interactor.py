from io import BytesIO

from common.interactor.base_interactor import BaseInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from common.repository.url_repository import UrlRepository
from postgres.models.db_models import File, FileTypeEnum
from postgres.models.external_models import File as FileExternal


class LoaderInteractor(BaseInteractor):

    def __init__(
        self,
        chat_repository: ChatRepository,
        file_sender_repository: FileSenderRepository,
        file_repository: FileRepository,
        url_repository: UrlRepository
    ):
        super(LoaderInteractor, self).__init__(chat_repository, url_repository)

        self.file_sender_repository = file_sender_repository
        self.file_repository = file_repository

    async def save_file(self, file_external: FileExternal, file: BytesIO):
        file_sender = await self.file_sender_repository.find_file_sender_by_id(file_external.sender_telegram_id)
        chat = await self.chat_repository.find_chat_by_telegram_id(file_external.chat_telegram_id)
        file_info: File = await self.file_repository.create_file_info(file_external, chat.Id, file_sender.Id)

        await self.file_repository.save_file(file, file_info.Id)

    async def save_url(self, url: str, sender_id: int, chat_id: int):
        name = self.url_repository.get_url_name(url)
        file_info = FileExternal(
            name=name,
            type=FileTypeEnum.Link,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id
        )
        file = BytesIO(bytes(url, encoding='utf-8'))

        await self.save_file(file_info, file)
