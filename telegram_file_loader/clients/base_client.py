from functools import partial
from typing import Optional
from urllib.parse import urljoin

from aiohttp import ClientSession
from ujson import dumps


class BaseClient:
    base_url: str = None
    raise_for_status: bool = True

    def get_session(self) -> ClientSession:
        return ClientSession(json_serialize=partial(dumps, reject_bytes=False))

    async def request(
            self,
            method: str,
            path: Optional[str] = None,
            **params,
    ) -> dict:
        full_url = urljoin(self.base_url, path)

        async with self.get_session().request(method=method, url=full_url, **params) as response:
            await response.read()

            if self.raise_for_status:
                response.raise_for_status()

            return await response.json()

    async def get(self, *args, **kwargs):
        return await self.request('GET', *args, **kwargs)

    async def post(self, *args, **kwargs):
        return await self.request('POST', *args, **kwargs)

    async def delete(self, *args, **kwargs):
        return await self.request('DELETE', *args, **kwargs)
