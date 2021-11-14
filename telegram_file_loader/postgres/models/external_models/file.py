import datetime

from postgres.models.db_models import FileTypeEnum
from pydantic import BaseModel, Field


class File(BaseModel):
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

    class Config:
        allow_population_by_field_name = True
