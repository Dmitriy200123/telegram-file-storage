import asyncio
import functools
import logging
import uuid

import peewee
from peewee_async import AsyncPostgresqlMixin, Manager
from playhouse.postgres_ext import IntegerField, Model, UUIDField
from playhouse.shortcuts import model_to_dict

__all__ = ['pg_db', 'manager', 'BaseModel', 'EnumField', 'atomic']

log = logging.getLogger('db.base')


class AsyncPGMixin(AsyncPostgresqlMixin):
    @property
    def connect_kwargs_async(self):
        """
        Connection parameters for `aiopg.Connection`
        """
        kwargs = self.connect_kwargs.copy()
        kwargs.update({
            'minsize': self.min_connections,
            'maxsize': self.max_connections,
            'enable_json': self._enable_json,
            'enable_hstore': self._enable_hstore,
        })
        if self.on_connect:
            kwargs['on_connect'] = self.on_connect
        if self.pool_recycle:
            kwargs['pool_recycle'] = self.pool_recycle

        return kwargs


class PooledPGDatabase(AsyncPGMixin, peewee.PostgresqlDatabase):

    def init(self, database, **kwargs):
        self.min_connections = kwargs.pop('min_connections', 1)
        self.max_connections = kwargs.pop('max_connections', 10)
        self.on_connect = kwargs.pop('on_connect', False)
        self.pool_recycle = kwargs.pop('pool_recycle', -1)
        super().init(database, **kwargs)
        self.init_async()


pg_db = PooledPGDatabase(None)
manager = Manager(pg_db)


def atomic():
    def _choose(func):
        if asyncio.iscoroutinefunction(func):
            @functools.wraps(func)
            async def _atomic(*args, **kwargs):
                async with manager.atomic():
                    return await func(*args, **kwargs)
        else:
            @functools.wraps(func)
            def _atomic(*args, **kwargs):
                with pg_db.atomic():
                    return func(*args, **kwargs)

        return _atomic

    return _choose


class BaseModel(Model):
    """
    Базовая модель с переопределенными функциями для удобства.

    В отличии от синхронной модели методы `create` и `get` являются асинхронными
    """
    Id = UUIDField(primary_key=True, default=uuid.uuid4)

    class Meta:
        database = pg_db

    def __repr__(self):
        return f'{self.__class__.__name__}:{self._get_pk_value()}'

    def as_dict(self):
        return model_to_dict(self, recurse=False)

    def is_changed(self, **kwargs):
        for name, value in kwargs.items():
            current_value = getattr(self, name)
            if current_value != value:
                return True
        return False

    async def update_instance(self, **params):
        """
        Функция обновления модели с проверкой надо ли обновлять в базе

        Returns:
            True/False в зависимости от того обновлялась ли модель в базе
        """
        new_params = {fld: val for fld,
                      val in params.items() if fld in self._meta.fields}
        changed = self.is_changed(**new_params)
        if changed:
            for fld, value in new_params.items():
                setattr(self, fld, value)
            await manager.update(self)
        return changed

    @classmethod
    async def find_by_name(cls, substring_name) -> list:
        if not substring_name:
            raise ValueError(f'Invalid name. Got {substring_name=}')
        query = cls.select().order_by(cls.Name, cls.Id).where(
            cls.Name.contains(substring_name))
        res = await manager.execute(query)
        return list(res)

    @classmethod
    async def create(cls, **kwargs):
        """
        Фунция создания экземпляра модели
        """
        async with manager.atomic():
            return await manager.create(cls, **kwargs)

    @classmethod
    async def get(cls, *args, **kwargs):
        """
        Фунция получения экземпляра модели
        """
        return await manager.get(cls, *args, **kwargs)


class EnumField(IntegerField):
    """Field for Enum value.

    Allow to use Enum values instead of string constants in code.
    """

    def __init__(self, enum, **kwargs):
        self._enum = enum
        self.value = None
        super().__init__(**kwargs)

    def db_value(self, value):
        assert isinstance(
            value, self._enum), f'Enum object {self._enum} expected, {type(value)} given'
        value = self._enum(value)
        return value.value

    def python_value(self, value):
        return self._enum(value)
