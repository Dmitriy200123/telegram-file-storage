import uuid

from peewee import BigIntegerField, CharField, UUIDField
from postgres.basic import BaseModel


class Chat(BaseModel):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)
    Name = CharField()
    ImageId = UUIDField(null=True)
    TelegramId = BigIntegerField(unique=True)

    class Meta:
        table_name = 'Chats'
