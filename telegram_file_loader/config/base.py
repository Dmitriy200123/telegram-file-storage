# Общие настройки
SUPPORTED_TEXT_FILE_TYPES = [
    'application/vnd.ms-powerpoint.slideshow.macroenabled.12',
    'application/vnd.ms-powerpoint.presentation.macroenabled.12',
    'application/msword', 'application/rtf',
    'application/vnd.oasis.opendocument.presentation',
    'application/vnd.ms-excel', 'text/plain',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'text/html',
    'application/vnd.openxmlformats-officedocument.presentationml.slideshow',
    'application/pdf',
    'application/vnd.openxmlformats-officedocument.presentationml.presentation',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'application/vnd.oasis.opendocument.spreadsheet',
    'application/vnd.oasis.opendocument.text',
    'application/mbox',
    'application/xml',
    'application/vnd.ms-powerpoint',
    # optional
    'application/vnd.ms-outlook',
    'application/vnd.apple.pages',
    'application/vnd.apple.keynote',
]

# Настройки базы
DB_HOST = 'localhost'
DB_PORT = 5432
DB_NAME = 'FileStorageApp'
DB_USER = 'FileStorageApp'
DB_PASS = 'change'
MAX_DB_CONNECTION = 10

# Настройка S3
S3_URL = 'http://localhost:4566'
BUCKET_NAME = 'test'
AWS_ACCESS_KEY_ID = '123'
AWS_SECRET_ACCESS_KEY = '123'
S3_URL_EXPIRES_IN_SECONDS = 60 * 60

# Telegram
API_ID = 13673260
API_HASH = '4c6c01da1fc915ffb51e4147bfae915c'
NUMBER = '+6287812030334'
