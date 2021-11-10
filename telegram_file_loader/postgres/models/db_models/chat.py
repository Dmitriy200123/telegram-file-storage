from peewee import BigIntegerField, CharField, UUIDField
from postgres.basic import BaseModel, manager


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
