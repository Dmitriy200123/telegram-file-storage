import datetime
from enum import Enum
from typing import Optional

from postgres.models.db_models import FileTypeEnum
from postgres.models.external_models.base import BaseExternalModel
from pydantic import Field, validator


class ExternalFileType(str, Enum):
    document = 'document'
    image = 'image'
    video = 'video'
    audio = 'audio'
    text_document = 'text_document'

    @classmethod
    def from_str(cls, value) -> 'ExternalFileType':
        if value == 'application' or value == 'text':
            return cls.document
        return cls(value)

    @classmethod
    def enum_map(cls) -> dict:
        return {
            cls.document: FileTypeEnum.Document,
            cls.image: FileTypeEnum.Image,
            cls.video: FileTypeEnum.Video,
            cls.audio: FileTypeEnum.Audio,
            cls.text_document: FileTypeEnum.TextDocument,
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

    @validator('type', pre=True)
    def type_converter(cls, value):  # noqa
        return ExternalFileType.enum_map()[value]

    def dict_non_empty_fields(self):
        return self.dict(by_alias=True, exclude_none=True, exclude={'sender_telegram_id', 'chat_telegram_id'})

    class Config:
        allow_population_by_field_name = True
