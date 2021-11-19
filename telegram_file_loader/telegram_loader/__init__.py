import config
import postgres
from clients.s3_client import S3Client
from pg_adapter import Adapter
from telethon import TelegramClient
from telegram_loader.loader.TelegramLoader import TelegramLoader
from telegram_loader.loader.TelegramSetting import TelegramSetting


async def start():
    client = TelegramClient('telegram_loader', config.API_ID, config.API_HASH)
    await TelegramSetting.configure_telegram_client(client, config.NUMBER)

    db_manager = postgres.start(max_connections=config.MAX_DB_CONNECTION)
    adapter = Adapter(db_manager)
    s3_client = S3Client(bucket_name='test')

    loader = TelegramLoader(client, adapter, s3_client)
    await loader.run()
