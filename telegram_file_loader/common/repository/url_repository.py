import logging

from aiohttp.client_exceptions import ClientError
from bs4 import BeautifulSoup
from clients.base_client import BaseClient
from postgres.models.external_models.telegram_models import ValidateUrl
from pydantic import AnyUrl
from urlextract import URLExtract

log = logging.getLogger(__name__)


class UrlRepository:
    PARSER_NAME = 'lxml'

    def __init__(self, url_extractor: URLExtract, http_client: BaseClient):
        self.url_extractor = url_extractor
        self.http_client = http_client

    def has_urls(self, message: str) -> bool:
        return self.url_extractor.has_urls(message)

    def find_urls(self, message: str) -> list[AnyUrl]:
        urls = self.url_extractor.find_urls(message)
        urls = list(map(lambda url: ValidateUrl(url=url).url, urls))
        return urls

    async def get_name(self, url: AnyUrl) -> str:
        try:
            html_data = await self.http_client.get(url)
            soup = BeautifulSoup(html_data, UrlRepository.PARSER_NAME)
            return soup.title.string
        except ClientError as exc:
            log.info(f'Get error in processing url. {exc=}')
            return url.host
