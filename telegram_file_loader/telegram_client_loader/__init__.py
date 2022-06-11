import config
from clients.base_client import BaseClient
from clients.documents_classifications_client import DocumentsClassificationsClient
from clients.documents_index_client import DocumentsIndexClient
from clients.documents_search_client import DocumentsSearchClient
from clients.s3_client import S3Client
from common.interactor.chat_interactor import ChatInteractor
from common.interactor.loader_interactor import LoaderInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from common.repository.sender_to_chat_repository import SenderToChatRepository
from common.repository.tag_repository import TagRepository
from common.repository.url_repository import UrlRepository
from postgres.pg_adapter import Adapter
from telegram_client_loader.handler.chat_handler import ChatHandler
from telegram_client_loader.handler.file_handler import FileHandler
from telegram_client_loader.setting.telegram_setting import TelegramSetting
from telethon import TelegramClient
from urlextract import URLExtract


async def start(
        pg_adapter: Adapter,
        s3_client: S3Client,
        documents_index_client: DocumentsIndexClient,
        documents_search_client: DocumentsSearchClient,
        documents_classifications_client: DocumentsClassificationsClient,
        http_client: BaseClient,
):
    url_extractor = URLExtract()

    telegram_client = TelegramClient(
        'telegram_client_loader',
        config.API_ID,
        config.API_HASH
    )
    await TelegramSetting.configure_telegram_client(telegram_client, config.NUMBER)

    chat_repository = ChatRepository(pg_adapter)
    file_sender_repository = FileSenderRepository(pg_adapter)
    file_repository = FileRepository(pg_adapter, s3_client)
    sender_to_chat_repository = SenderToChatRepository(pg_adapter)
    url_repository = UrlRepository(url_extractor, http_client)
    tag_repository = TagRepository(pg_adapter)

    loader_interactor = LoaderInteractor(
        chat_repository=chat_repository,
        file_repository=file_repository,
        file_sender_repository=file_sender_repository,
        url_repository=url_repository,
        tag_repository=tag_repository,
        documents_index_client=documents_index_client,
        documents_search_client=documents_search_client,
        documents_classifications_client=documents_classifications_client,
    )
    chat_interactor = ChatInteractor(
        chat_repository=chat_repository,
        file_sender_repository=file_sender_repository,
        file_repository=file_repository,
        sender_to_chat_repository=sender_to_chat_repository,
        url_repository=url_repository,
        tag_repository=tag_repository
    )

    loader = FileHandler(
        telegram_client=telegram_client,
        loader_interactor=loader_interactor,
        chat_interactor=chat_interactor,
    )
    chat_handler = ChatHandler(telegram_client, chat_interactor)

    return telegram_client
