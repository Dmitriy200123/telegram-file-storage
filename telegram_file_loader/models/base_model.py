import uuid

from common.postgres.basic import BaseModel as Model
from common.postgres.basic import manager
from peewee import UUIDField


class BaseModel(Model):
    Id = UUIDField(primary_key=True, default=uuid.uuid4)

    @classmethod
    async def find_by_name(cls, substring_name) -> list:
        if not substring_name:
            raise ValueError(f'Invalid name. Got {substring_name=}')
        query = cls.select().order_by(cls.Name, cls.Id).where(
            cls.Name.contains(substring_name))
        res = await manager.execute(query)
        return list(res)
