from telethon import TelegramClient


class TelegramSetting:

    @staticmethod
    async def configure_telegram_client(client: TelegramClient, number: str):
        await client.connect()

        if not await client.is_user_authorized():
            await client.send_code_request(phone=number)
            code = TelegramSetting.get_code()
            await client.sign_in(phone=number, code=code)

    @staticmethod
    def get_code() -> str:
        return input('Enter code: ')
