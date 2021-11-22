from peewee import ForeignKeyField
from postgres.basic import BaseModel
from postgres.models.db_models import Chat, FileSender


class SenderToChat(BaseModel):
    SenderId = ForeignKeyField(model=FileSender, db_column='SendersId')
    ChatId = ForeignKeyField(model=Chat, db_column='ChatsId')

    class Meta:
        table_name = 'SenderAndChat'
