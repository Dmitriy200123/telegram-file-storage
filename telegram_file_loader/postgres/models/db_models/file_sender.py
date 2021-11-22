import uuid

from peewee import BigIntegerField, CharField, UUIDField
from postgres.basic import BaseModel


class FileSender(BaseModel):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)
    TelegramId = BigIntegerField()
    TelegramUserName = CharField()
    FullName = CharField()

    class Meta:
        table_name = 'FileSenders'
