import dataclasses

import config
from clients.base_client import BaseClient


@dataclasses.dataclass
class SearchDocumentsClient(BaseClient):
    base_url: str = config.SEARCH_DOCUMENT_URL
    index_url = base_url + '/index'

    async def index_document(self, document_id: str, name: str, content: bytes):
        payload = {
            'id': document_id,
            'name': name,
            'content': content,
        }

        return await self.post(path=f'{self.index_url}/documents', json=payload)
