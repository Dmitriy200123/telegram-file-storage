import datetime

from postgres.models.db_models import FileTypeEnum
from pydantic import UUID4, BaseModel, Field


class Base(BaseModel):
    class Config:
        allow_population_by_field_name = True


class Chat(Base):
    name: str = Field(..., tittle='Название чата',
                      max_length=255, alias='Name')
    image_id: UUID4 = Field(..., title='Id картинки чата', alias='ImageId')
    telegram_id: int = Field(..., title='Телеграм id', alias='TelegramId')


class FileSender(Base):
    telegram_id: int = Field(..., title='Телеграм id', alias='TelegramId')
    telegram_username: str = Field(
        ..., title='Имя пользователя в телеграме', alias='TelegramUserName')
    full_name: str = Field(..., title='Полное имя пользователя в телеграме',
                           max_length=255, alias='FullName')


class File(Base):
    name: str = Field(..., title='Название файла',
                      max_length=255, alias='Name')
    extension: str = Field(..., title='Расширение',
                           max_length=255, alias='Extension')
    type: FileTypeEnum = Field(..., title='Тип', alias='TypeId')
    upload_date: datetime.datetime = Field(...,
                                           title='Дата загрузки', alias='UploadDate')
    file_sender_id: str = Field(...,
                                title='Телеграм id отправителя', alias='FileSenderId')
    chat_id: str = Field(..., title='', alias='Телеграм id чата')
