import asyncio
import sys

import config
import postgres
import telegram_client_loader
from clients.s3_client import S3Client
from postgres.pg_adapter import Adapter
from telethon import functions


async def init():
    db_manager = postgres.start(max_connections=config.MAX_DB_CONNECTION)
    adapter = Adapter(db_manager)
    s3_client = S3Client(bucket_name=config.BUCKET_NAME)
    telegram_client = await telegram_client_loader.start(pg_adapter=adapter, s3_client=s3_client)

    if config.TEST_MODE:
        dialogs = await telegram_client.get_dialogs()

        if not any([chat for chat in dialogs if chat.name == config.TEST_CHAT_NAME]):
            chat = await telegram_client(functions.messages.CreateChatRequest(
                users=config.TEST_USERS_TO_CREATE_CHAT,
                title=config.TEST_CHAT_NAME
            ))

            print(chat)


def main():
    if sys.platform == 'win32':
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())

    loop = asyncio.get_event_loop()
    try:
        loop.run_until_complete(init())
        loop.run_forever()
    except KeyboardInterrupt:
        pass


if __name__ == '__main__':
    main()
