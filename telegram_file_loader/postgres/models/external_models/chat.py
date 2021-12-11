from typing import Optional

from pydantic import UUID4, BaseModel, Field


class Chat(BaseModel):
    name: str = Field(..., tittle='Название чата',
                      max_length=255, alias='Name')
    image_id: Optional[UUID4] = Field(
        title='Id картинки чата', alias='ImageId')
    telegram_id: int = Field(..., title='Телеграм id', alias='TelegramId')

    class Config:
        allow_population_by_field_name = True
