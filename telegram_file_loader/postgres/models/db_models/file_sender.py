from peewee import BigIntegerField, CharField
from postgres.basic import BaseModel


class FileSender(BaseModel):
    TelegramId = BigIntegerField()
    TelegramUserName = CharField()
    FullName = CharField()

    class Meta:
        table_name = 'FileSender'
