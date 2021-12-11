from urllib.error import URLError
from urllib.request import urlopen

from bs4 import BeautifulSoup
from common import utils
from common.repository.base_repository import BaseRepository
from postgres.pg_adapter import Adapter
from urlextract import URLExtract


class UrlRepository(BaseRepository):

    PARSER_NAME = 'lxml'
    HTTP = 'http://'

    def __init__(self, adapter: Adapter, url_extractor: URLExtract):
        super(UrlRepository, self).__init__(adapter)
        self.url_extractor = url_extractor

    def has_urls(self, message: str) -> bool:
        return self.url_extractor.has_urls(message)

    def find_urls(self, message: str) -> list[str]:
        return self.url_extractor.find_urls(message)

    @staticmethod
    def get_url_name(url: str) -> str:
        if not url.startswith('http'):
            url = f'{UrlRepository.HTTP}{url}'

        try:
            html_data = urlopen(url)
            soup = BeautifulSoup(html_data, UrlRepository.PARSER_NAME)
            return soup.title.string
        except URLError:
            return utils.domain_name(url)
