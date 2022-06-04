import dataclasses
import urllib.parse
from typing import List

import config
from clients.base_client import BaseClient


@dataclasses.dataclass
class DocumentsSearchClient(BaseClient):
    base_url: str = config.SEARCH_DOCUMENT_URL
    search_url = base_url + '/search'

    async def contains_in_name(self, document_id: str, queries: List[str]):
        """
        Проверяет, есть ли в названии документа что-то из списка строк

        Returns: "true" или "false"

        """
        query = urllib.parse.urlencode({'queries': param for param in queries})

        return await self.get(path=f'{self.search_url}/documents/{document_id}/containsInName?{query}')
