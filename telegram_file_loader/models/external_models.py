from pydantic import UUID4, BaseModel, Field


class Chat(BaseModel):
    name: str = Field(..., tittle='Название чата', alias='Name')
    # само валидирует на то, норм ли ууид
    image_id: UUID4 = Field(..., title='id картинки чата', alias='ImageId')
    telegram_id: int = Field(..., title='телеговский id', alias='TelegramId')

    class Config:
        allow_population_by_field_name = True
