from urllib.error import URLError
from urllib.request import urlopen

from bs4 import BeautifulSoup
from postgres.models.external_models.telegram_models import ValidateUrl
from pydantic import AnyUrl
from urlextract import URLExtract


class UrlRepository:
    PARSER_NAME = 'lxml'

    def __init__(self, url_extractor: URLExtract):
        self.url_extractor = url_extractor

    def has_urls(self, message: str) -> bool:
        return self.url_extractor.has_urls(message)

    def find_urls(self, message: str) -> list[AnyUrl]:
        urls = self.url_extractor.find_urls(message)
        urls = list(map(lambda url: ValidateUrl(url=url).url, urls))
        return urls

    @staticmethod
    def get_name(url: AnyUrl) -> str:
        try:
            html_data = urlopen(url)
            soup = BeautifulSoup(html_data, UrlRepository.PARSER_NAME)
            return soup.title.string
        except URLError:
            return url.host
