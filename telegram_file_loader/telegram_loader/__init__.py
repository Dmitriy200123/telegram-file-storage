import config
from telethon import TelegramClient
from telegram_loader.loader.TelegramLoader import TelegramLoader
from telegram_loader.loader.TelegramSetting import TelegramSetting


async def start():
    client = TelegramClient('telegram_loader', config.API_ID, config.API_HASH)
    await TelegramSetting.configure_telegram_client(client, config.NUMBER)

    loader = TelegramLoader(client)
    await loader.run()
