import asyncio
import sys

import postgres
import telegram_client_loader


async def init():
    postgres.start()
    await telegram_client_loader.start()


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
