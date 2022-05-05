import dataclasses
from base64 import b64encode
from io import BytesIO

import config
from clients.base_client import BaseClient


@dataclasses.dataclass
class DocumentsIndexClient(BaseClient):
    base_url: str = config.DOCUMENTS_API_URL
    index_url = base_url + '/index'

    async def index_document(self, document_id: str, name: str, content: BytesIO):
        # суровое и беспощадное низкоуровневое программирование на питоне
        content.seek(0)

        payload = {
            'id': document_id,
            'name': name,
            'content': b64encode(content.read()),
        }

        return await self.post(path=f'{self.index_url}/documents', json=payload)
