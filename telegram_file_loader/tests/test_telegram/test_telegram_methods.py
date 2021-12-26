from postgres.models.db_models import Code
from telegram_client_loader.setting.telegram_setting import TelegramSetting


async def test_get_none_code():
    code = await TelegramSetting.get_code_from_db()
    assert not code


async def test_get_code():
    secret = 'super_secret_code'
    await Code.create(code=secret)
    code = await TelegramSetting.get_code_from_db()
    assert code == secret
