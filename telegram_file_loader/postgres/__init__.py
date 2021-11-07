import logging
from time import sleep

import config as config
import peewee
from postgres import basic

log = logging.getLogger('db.init')


def start(max_connections=10):
    """
    Основная функция инициализации подключения к БД

    Создает пул асинхронных коннектов (не более 20 коннектов в пуле)
    """
    for i in range(10):
        try:
            basic.pg_db.init(
                database=config.DB_NAME,
                user=config.DB_USER,
                password=config.DB_PASS,
                host=config.DB_HOST,
                port=config.DB_PORT,
                max_connections=max_connections,
                pool_recycle=300
            )
            basic.pg_db.connect()
        except peewee.OperationalError:
            if i == 9:
                raise
            log.info('DB unavailable, just try another one')
            sleep(1)
            continue
        else:
            break
    return basic.manager
