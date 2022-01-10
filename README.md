# Telegram file storage
**Web-сервис:**
* автоматический сбор файлов из telegram
* быстрый и удобный поиск файлов по фильтрам

**В основе приложения лежат три основных сервиса:**
* Backend на ASP NET CORE
* Frontend на React + TS
* Автозагрузчик файлов из telegram на Python(telethon)

**Хранилища:** PostgreSQL и S3.

На API и взаимодейcвие с хранилищами написаны модульные и интеграционный тесты.

**Cредство развертывания:** Docker.

**Дополнительная информация о проекте:** https://docs.google.com/document/d/1SwMOaDpWGentIMgjLpeoOnelNBQsUNKo-8OMZnc7q_c/edit?usp=sharing

## Локальная разработка
В корне проекта (/FileStorageApp) находится `docker-compose-dev` файл, описывающий развертку всех необходимых сервисов для запуска на локальной машине

Запуск конейнеров aws s3 и PostgreSQL:` docker-compose -f docker-compose-dev.yml up --build --force-recreate postgres s3`

## Боевой запуск
`docker-compose up`
и после этого ввести код из telegram, сохраненный в PostreSQL

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

