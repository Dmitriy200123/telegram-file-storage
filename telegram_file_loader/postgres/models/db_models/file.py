import uuid
from enum import Enum

from peewee import CharField, DateTimeField, ForeignKeyField, UUIDField
from postgres.basic import BaseModel, EnumField
from postgres.models.db_models.chat import Chat
from postgres.models.db_models.file_sender import FileSender


class FileTypeEnum(int, Enum):
    Document = 0
    Audio = 1
    Video = 2
    Image = 3


class File(BaseModel):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)
    Name = CharField()
    Extension = CharField()
    TypeId = EnumField(enum=FileTypeEnum, null=False)
    UploadDate = DateTimeField()
    FileSenderId = ForeignKeyField(model=FileSender, db_column='FileSenderId')
    ChatId = ForeignKeyField(model=Chat, db_column='ChatId')

    class Meta:
        table_name = 'Files'
