import config
import postgres
from clients.s3_client import S3Client
from common.interactor.chat_interactor import ChatInteractor
from common.interactor.loader_interactor import LoaderInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from common.repository.sender_to_chat_repository import SenderToChatRepository
from common.repository.url_repository import UrlRepository
from postgres.pg_adapter import Adapter
from telegram_client_loader.handler.chat_handler import ChatHandler
from telegram_client_loader.loader.telegram_loader import TelegramLoader
from telegram_client_loader.setting.telegram_setting import TelegramSetting
from telethon import TelegramClient
from urlextract import URLExtract


async def start():
    telegram_client = TelegramClient(
        'telegram_client_loader',
        config.API_ID,
        config.API_HASH
    )
    await TelegramSetting.configure_telegram_client(telegram_client, config.NUMBER)

    db_manager = postgres.start(max_connections=config.MAX_DB_CONNECTION)
    adapter = Adapter(db_manager)
    s3_client = S3Client(bucket_name=config.BUCKET_NAME)
    url_extractor = URLExtract()

    chat_repository = ChatRepository(adapter)
    file_sender_repository = FileSenderRepository(adapter)
    file_repository = FileRepository(adapter, s3_client)
    sender_to_chat_repository = SenderToChatRepository(adapter)
    url_repository = UrlRepository(adapter, url_extractor)

    loader_interactor = LoaderInteractor(
        chat_repository=chat_repository,
        file_repository=file_repository,
        file_sender_repository=file_sender_repository,
        url_repository=url_repository
    )
    chat_interactor = ChatInteractor(
        chat_repository=chat_repository,
        file_sender_repository=file_sender_repository,
        file_repository=file_repository,
        sender_to_chat_repository=sender_to_chat_repository,
        url_repository=url_repository
    )

    loader = TelegramLoader(
        telegram_client,
        loader_interactor,
        chat_interactor
    )
    chat_handler = ChatHandler(telegram_client, chat_interactor)
