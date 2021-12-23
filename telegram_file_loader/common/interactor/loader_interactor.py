from io import BytesIO

from common.interactor.base_interactor import BaseInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from common.repository.tag_repository import TagRepository
from common.repository.url_repository import UrlRepository
from postgres.models.db_models import Chat, File, FileSender, FileTypeEnum
from postgres.models.external_models import File as FileExternal


class LoaderInteractor(BaseInteractor):
    DEFAULT_ENCODE = 'utf-8'

    def __init__(
        self,
        chat_repository: ChatRepository,
        file_sender_repository: FileSenderRepository,
        file_repository: FileRepository,
        url_repository: UrlRepository,
        tag_repository: TagRepository
    ):
        super(LoaderInteractor, self).__init__(
            chat_repository, url_repository, tag_repository)

        self.file_sender_repository = file_sender_repository
        self.file_repository = file_repository

    async def save_file(self, file_external: FileExternal, file: BytesIO):
        file_sender: FileSender = await self.file_sender_repository \
            .find_by_telegram_id(file_external.sender_telegram_id)
        chat: Chat = await self.chat_repository.find_by_telegram_id(file_external.chat_telegram_id)
        file_info: File = await self.file_repository.create_or_get(file_external, chat.Id, file_sender.Id)

        await self.file_repository.save_file(file, file_info.Id)

    async def save_url(self, url: str, sender_id: int, chat_id: int):
        name: str = self.url_repository.get_name(url)
        file_info: FileExternal = FileExternal(
            name=name,
            type=FileTypeEnum.Link,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id
        )
        file: BytesIO = BytesIO(self.__text_to_bytes(url))

        await self.save_file(file_info, file)

    async def save_text(self, title: str, description: str, sender_id: int, chat_id: int):
        file_info: FileExternal = FileExternal(
            name=title,
            type=FileTypeEnum.Text,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id
        )
        file: BytesIO = BytesIO(self.__text_to_bytes(description))

        await self.save_file(file_info, file)

    @staticmethod
    def __text_to_bytes(text: str) -> bytes:
        return bytes(text, encoding=LoaderInteractor.DEFAULT_ENCODE)
