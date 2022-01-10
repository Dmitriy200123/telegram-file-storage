import uuid

from peewee import CharField, UUIDField
from postgres.basic import BaseModel


class MarkedTextTags(BaseModel):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)
    TitleTag = CharField()
    DescriptionTag = CharField()

    class Meta:
        table_name = 'MarkedTextTags'
