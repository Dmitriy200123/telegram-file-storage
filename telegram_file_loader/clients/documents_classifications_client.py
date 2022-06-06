import dataclasses
import urllib.parse
from typing import List

import config
import ujson
from clients.base_client import BaseClient
from postgres.models.internal_models import Classification


@dataclasses.dataclass
class DocumentsClassificationsClient(BaseClient):
    base_url: str = config.DOCUMENT_CLASSIFICATIONS_API_URL
    headers: dict = dataclasses.field(
        default_factory=lambda: {
            'Authorization': f'Bearer {config.DOCUMENT_CLASSIFICATIONS_API_AUTH_TOKEN}'}
    )

    async def get_classifications(self, skip=0, take=config.DOCUMENT_CLASSIFICATIONS_API_DEFAULT_TAKE):
        query = urllib.parse.urlencode(
            {
                'skip': skip,
                'take': take,
                'includeClassificationWords': config.DOCUMENT_CLASSIFICATIONS_API_INCLUDE_WORDS
            }
        )
        response = await self.get(path=f'?{query}', headers=self.headers)
        response_json: List = ujson.loads(response)
        return [Classification(**classification) for classification in response_json]
