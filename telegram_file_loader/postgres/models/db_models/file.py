from enum import Enum

from peewee import CharField, ForeignKeyField, TimestampField
from postgres.basic import BaseModel, EnumField
from postgres.models.db_models.chat import Chat
from postgres.models.db_models.file_sender import FileSender


class FileTypeEnum(int, Enum):
    Document = 0
    Audio = 1
    Video = 2
    Image = 3


class File(BaseModel):
    Name = CharField()
    Extension = CharField()
    TypeId = EnumField(enum=FileTypeEnum, null=False)
    UploadDate = TimestampField()
    FileSenderId = ForeignKeyField(model=FileSender)
    ChatId = ForeignKeyField(model=Chat)

    class Meta:
        table_name = 'Files'
