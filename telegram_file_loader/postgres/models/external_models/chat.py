from typing import Optional

from postgres.models.external_models.base import BaseExternalModel
from pydantic import UUID4, Field


class Chat(BaseExternalModel):
    name: str = Field(..., tittle='Название чата',
                      max_length=255, alias='Name')
    image_id: Optional[UUID4] = Field(
        title='Id картинки чата', alias='ImageId')
    telegram_id: int = Field(..., title='Телеграм id', alias='TelegramId')

    class Config:
        allow_population_by_field_name = True
