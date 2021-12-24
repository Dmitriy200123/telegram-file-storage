import asyncio
from typing import List, Union

from postgres.basic import manager
from postgres.models.db_models import Code
from telethon import TelegramClient


class TelegramSetting:

    @staticmethod
    async def configure_telegram_client(client: TelegramClient, number: str):
        await client.connect()

        if not await client.is_user_authorized():
            Code.truncate_table()
            await client.send_code_request(phone=number)
            code = await TelegramSetting.get_code()
            await client.sign_in(phone=number, code=code)

    @staticmethod
    async def get_code() -> str:
        while True:
            if code := await TelegramSetting.get_code_from_db():
                return code
            await asyncio.sleep(2)

    @staticmethod
    async def get_code_from_db() -> Union[str, None]:
        result: List[Code] = list(await manager.execute(Code.select()))
        if len(result) != 0:
            code = result[0].code
            Code.truncate_table()
            return code
