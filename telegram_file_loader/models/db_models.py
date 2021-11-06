from enum import Enum

from common.postgres.basic import EnumField, manager
from models.base_model import BaseModel
from peewee import (
    BigIntegerField,
    CharField,
    ForeignKeyField,
    TimestampField,
    UUIDField,
)

__all__ = ['Chat', 'FileSender', 'File', 'FileTypeEnum']


class Chat(BaseModel):
    Name = CharField()
    ImageId = UUIDField()
    TelegramId = BigIntegerField()

    @classmethod
    async def find_by_telegram_id(cls, telegram_id: int) -> list:
        query = cls.select().order_by(cls.Name, cls.Id).where(
            cls.TelegramId == telegram_id)
        res = await manager.execute(query)
        return list(res)

    class Meta:
        table_name = 'Chats'


class FileSender(BaseModel):
    TelegramId = BigIntegerField()
    TelegramUserName = CharField()
    FullName = CharField()

    class Meta:
        table_name = 'FileSender'


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
