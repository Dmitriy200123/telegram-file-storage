import os

# Настройки окружения
ENV = os.getenv('ENVIRONMENT', 'not_set')
VERSION = os.getenv('TAG', 'not_set')

# Настройки базы
DB_HOST = 'pg-prod-url'
DB_PORT = 5432
DB_NAME = 'FileStorageApp'
DB_USER = 'FileStorageApp'
DB_PASS = 'change'
MAX_DB_CONNECTION = 10
