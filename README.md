# Telegram file storage

## Локальная разработка
В корне проекта (/FileStorageApp) находится `docker-compose` файл, описывающий развертку всех необходимых сервисов для запуска на локальной машине

Запуск конейнеров aws s3 и PostgreSQL:` docker-compose up --build --force-recreate postgres s3`

###Данные для подключения локально:

#### S3
    host: localhost
    port: 4566

#### PostgreSQL
    host: localhost
    port: 5432
    user: FileStorageApp
    dbName: FileStorageApp
    password: change
