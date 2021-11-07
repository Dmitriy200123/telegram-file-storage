import datetime

from db_models import FileTypeEnum
from pydantic import UUID4, BaseModel, Field


class Base(BaseModel):
    class Config:
        allow_population_by_field_name = True


class Chat(Base):
    name: str = Field(..., tittle='Название чата',
                      max_length=255, alias='Name')
    image_id: UUID4 = Field(..., title='id картинки чата', alias='ImageId')
    telegram_id: int = Field(..., title='телеговский id', alias='TelegramId')


class FileSender(Base):
    telegram_id: int = Field(..., title='', alias='TelegramId')
    telegram_username: str = Field(..., title='', alias='TelegramUserName')
    full_name: str = Field(..., title='', max_length=255, alias='FullName')


class File(Base):
    name: str = Field(..., title='', max_length=255, alias='Name')
    extension: str = Field(..., title='', max_length=255, alias='Extension')
    type: FileTypeEnum = Field(..., title='', alias='TypeId')
    upload_date: datetime.datetime = Field(..., title='', alias='UploadDate')
    file_sender_id: str = Field(..., title='', alias='FileSenderId')
    chat_id: str = Field(..., title='', alias='ChatId')
