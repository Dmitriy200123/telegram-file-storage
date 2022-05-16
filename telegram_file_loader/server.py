import asyncio
import logging
import sys

import config
import postgres
import telegram_client_loader
from clients.documents_classifications_client import DocumentsClassificationsClient
from clients.documents_index_client import DocumentsIndexClient
from clients.documents_search_client import DocumentsSearchClient
from clients.s3_client import S3Client
from postgres.pg_adapter import Adapter

log = logging.getLogger(__name__)


async def init():
    db_manager = postgres.start(max_connections=config.MAX_DB_CONNECTION)
    adapter = Adapter(db_manager)
    s3_client = S3Client(bucket_name=config.BUCKET_NAME)
    documents_index_client = DocumentsIndexClient()
    documents_search_client = DocumentsSearchClient()
    documents_classifications_client = DocumentsClassificationsClient()

    await telegram_client_loader.start(
        pg_adapter=adapter,
        s3_client=s3_client,
        documents_index_client=documents_index_client,
        documents_search_client=documents_search_client,
        documents_classifications_client=documents_classifications_client,
    )

    log.info('loaded success')


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
