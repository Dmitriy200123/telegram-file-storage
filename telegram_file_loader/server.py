import asyncio
import sys
import telegram_loader


async def init():
    await telegram_loader.start()


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
