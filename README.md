# Telegram file storage

## Локальная разработка
В корне проекта (/FileStorageApp) находится `docker-compose-dev` файл, описывающий развертку всех необходимых сервисов для запуска на локальной машине

Запуск конейнеров aws s3 и PostgreSQL:` docker-compose -f docker-compose-dev.yml up --build --force-recreate postgres s3`

## Боевой запуск
`docker-compose up`
и после этого ввести кодик из телеги в табличку в базе

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
