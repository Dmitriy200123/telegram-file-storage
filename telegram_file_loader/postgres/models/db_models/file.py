import uuid
from datetime import datetime
from enum import Enum

from peewee import CharField, DateTimeField, ForeignKeyField, UUIDField
from postgres.basic import BaseModel, EnumField
from postgres.models.db_models.chat import Chat
from postgres.models.db_models.file_sender import FileSender


class DocumentClassifications(BaseModel):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)
    Name = CharField()
    CreatedAt = DateTimeField()

    class Meta:
        table_name = 'DocumentClassifications'


class FileTypeEnum(int, Enum):
    Document = 0
    Audio = 1
    Video = 2
    Image = 3
    Link = 4
    Text = 5
    TextDocument = 6


class File(BaseModel):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)
    Name = CharField()
    Extension = CharField(null=True)
    TypeId = EnumField(enum=FileTypeEnum, null=False)
    UploadDate = DateTimeField(default=datetime.now)
    FileSenderId = ForeignKeyField(model=FileSender, db_column='FileSenderId')
    ChatId = ForeignKeyField(model=Chat, null=True, db_column='ChatId')
    ClassificationId = ForeignKeyField(
        model=DocumentClassifications, null=True, db_column='ClassificationId')

    class Meta:
        table_name = 'Files'
