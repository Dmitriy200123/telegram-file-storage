import datetime
from typing import Optional

from postgres.models.db_models import FileTypeEnum
from postgres.models.external_models.base import BaseExternalModel
from pydantic import Field

type_map = {
    'document': FileTypeEnum.Document,
    'image': FileTypeEnum.Image,
    'video': FileTypeEnum.Video,
    'audio': FileTypeEnum.Audio
}


class File(BaseExternalModel):
    name: str = Field(..., title='Название файла',
                      max_length=255, alias='Name')
    extension: Optional[str] = Field(
        title='Расширение', max_length=255, alias='Extension')
    type: FileTypeEnum = Field(..., title='Тип', alias='TypeId')
    upload_date: Optional[datetime.datetime] = Field(
        title='Дата загрузки', alias='UploadDate')

    sender_telegram_id: int
    chat_telegram_id: int

    def dict_non_empty_fields(self):
        return self.dict(by_alias=True, exclude_none=True, exclude={'sender_telegram_id', 'chat_telegram_id'})

    class Config:
        allow_population_by_field_name = True
