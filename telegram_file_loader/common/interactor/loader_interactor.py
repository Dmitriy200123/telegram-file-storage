import logging
from io import BytesIO

import config
from aiohttp.client_exceptions import ClientResponseError
from clients.documents_classifications_client import DocumentsClassificationsClient
from clients.documents_index_client import DocumentsIndexClient
from clients.documents_search_client import DocumentsSearchClient
from common.interactor.base_interactor import BaseInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from common.repository.tag_repository import TagRepository
from common.repository.url_repository import UrlRepository
from postgres.models.db_models import Chat, File, FileSender, FileTypeEnum
from postgres.models.external_models import File as FileExternal
from pydantic import AnyUrl

log = logging.getLogger(__name__)


class LoaderInteractor(BaseInteractor):
    DEFAULT_ENCODE = 'utf-8'

    def __init__(
            self,
            chat_repository: ChatRepository,
            file_sender_repository: FileSenderRepository,
            file_repository: FileRepository,
            url_repository: UrlRepository,
            tag_repository: TagRepository,
            documents_index_client: DocumentsIndexClient,
            documents_search_client: DocumentsSearchClient,
            documents_classifications_client: DocumentsClassificationsClient,
    ):
        super(LoaderInteractor, self).__init__(
            chat_repository, url_repository, tag_repository)

        self.file_sender_repository = file_sender_repository
        self.file_repository = file_repository
        self.documents_index_client = documents_index_client
        self.documents_search_client = documents_search_client
        self.documents_classifications_client = documents_classifications_client

    async def save_file(self, file_external: FileExternal, file: BytesIO, mime_type=None):
        file_sender: FileSender = await self.file_sender_repository \
            .find_by_telegram_id(file_external.sender_telegram_id)
        chat: Chat = await self.chat_repository.find_by_telegram_id(file_external.chat_telegram_id)
        file_info: File = await self.file_repository.create_or_get(file_external, chat.Id, file_sender.Id)

        uuid = await self.file_repository.save_file(file, file_info.Id, mime_type)
        str_uuid = str(uuid)

        if file_external.type is FileTypeEnum.TextDocument:
            try:
                await self.documents_index_client.index_document(
                    document_id=str_uuid,
                    name=file_external.name,
                    content=file
                )
            except ClientResponseError:
                log.info("Can't index document. Skip classification")
                return

            step = 0
            classifications = await self.documents_classifications_client.get_classifications()
            while len(classifications) != 0:
                for classification in classifications:
                    search_result = await self.documents_search_client.contains_in_name(
                        document_id=str_uuid,
                        queries=[classification_word.value for classification_word in
                                 classification.classificationWords]
                    )
                    if search_result == 'true':
                        await file_info.update_instance(ClassificationId=classification.id)
                        return

                step += 1
                classifications = await self.documents_classifications_client.get_classifications(
                    skip=step * config.DOCUMENT_CLASSIFICATIONS_API_DEFAULT_TAKE
                )

            log.info('Not found any classifications')

    async def save_url(self, url: AnyUrl, sender_id: int, chat_id: int):
        name = await self.get_url_name(url)
        file_info: FileExternal = FileExternal(
            name=name,
            type=FileTypeEnum.Link,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id
        )
        file: BytesIO = BytesIO(self.__text_to_bytes(url.title().lower()))

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
