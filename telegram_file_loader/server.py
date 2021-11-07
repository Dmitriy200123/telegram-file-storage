import asyncio

import config
from common import db


async def init():
    db.start(max_connections=config.MAX_DB_CONNECTION)


def main():
    loop = asyncio.get_event_loop()
    try:
        loop.run_until_complete(init())
        loop.run_forever()
    except KeyboardInterrupt:
        pass


if __name__ == '__main__':
    main()
