import config
import postgres
from clients.s3_client import S3Client
from common.interactor.loader_interactor import LoaderInteractor
from common.repository.chat_repository import ChatRepository
from common.repository.file_repository import FileRepository
from common.repository.file_sender_repository import FileSenderRepository
from postgres.pg_adapter import Adapter
from telegram_client_loader.handler.chat_handler import ChatHandler
from telegram_client_loader.loader.telegram_loader import TelegramLoader
from telegram_client_loader.setting.telegram_setting import TelegramSetting
from telethon import TelegramClient


async def start():
    telegram_client = TelegramClient(
        'telegram_client_loader', config.API_ID, config.API_HASH)
    await TelegramSetting.configure_telegram_client(telegram_client, config.NUMBER)

    db_manager = postgres.start(max_connections=config.MAX_DB_CONNECTION)
    adapter = Adapter(db_manager)
    s3_client = S3Client(bucket_name=config.BUCKET_NAME)

    chat_repository = ChatRepository(adapter)
    file_sender_repository = FileSenderRepository(adapter)
    file_repository = FileRepository(adapter, s3_client)

    loader_interactor = LoaderInteractor(
        chat_repository=chat_repository,
        file_repository=file_repository,
        file_sender_repository=file_sender_repository
    )

    loader = TelegramLoader(telegram_client, loader_interactor)  # noqa
    chat_handler = ChatHandler(telegram_client)  # noqa
