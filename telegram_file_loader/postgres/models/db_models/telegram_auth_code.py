from peewee import CharField
from postgres.basic import BaseModel


class Code(BaseModel):
    code = CharField(verbose_name='Код для авторизации Telethon')

    class Meta:
        table_name = 'Code'
