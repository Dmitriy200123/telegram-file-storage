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

# Настройка S3
S3_URL = 'http://localhost:4566'
AWS_ACCESS_KEY_ID = '123'
AWS_SECRET_ACCESS_KEY = '123'
