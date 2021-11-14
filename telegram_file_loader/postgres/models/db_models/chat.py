from peewee import BigIntegerField, CharField, UUIDField
from postgres.basic import BaseModel


class Chat(BaseModel):
    Name = CharField()
    ImageId = UUIDField()
    TelegramId = BigIntegerField()

    class Meta:
        table_name = 'Chats'
