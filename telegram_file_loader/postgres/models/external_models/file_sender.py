from postgres.models.external_models.base import BaseExternalModel
from pydantic import Field


class FileSender(BaseExternalModel):
    telegram_id: int = Field(..., title='Телеграм id', alias='TelegramId')
    telegram_username: str = Field(
        ..., title='Имя пользователя в телеграме', alias='TelegramUserName')
    full_name: str = Field(..., title='Полное имя пользователя в телеграме',
                           max_length=255, alias='FullName')

    class Config:
        allow_population_by_field_name = True
