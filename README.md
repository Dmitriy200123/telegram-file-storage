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
В корне проекта находится `docker-compose-dev` файл, описывающий развертку всех необходимых сервисов для запуска на локальной машине

Основная команда для запука: `docker-compose -f docker-compose-dev.yml up --build --force-recreate`
Поднимет все сервисы приложения.

Для запуска отдельных сервисов можно использовать команду: ` docker-compose -f docker-compose-dev.yml up --build --force-recreate <container-name1> <container-name2> <...>`

Список контейнеров: 
 - **s3** - Хранилище S3
 - **postgres** - БД
 - **dotnet_backend** - Основной backend приложения
 - **front** - Frontend часть приложения
 - **telegram_auth** - Backend часть, отвечающая за авторизацию
 - **telegram_loader** - Backend часть, отвечающая за коммуникацию с Telegram и загрузкой файлов
 - **elasticsearch** - Поисковая система ElasticSearch
 - **kibana** - Интерйефс для взаимодействия с ElasticSearch


### Примеры запуска для локальной разработки
 - `docker-compose -f docker-compose-dev.yml up --build --force-recreate s3 postgres telegram_auth elasticsearch` - Запустит все для локальной разработки backend части
   - `docker-compose -f docker-compose-dev.yml up --build --force-recreate s3 postgres telegram_auth elasticsearch telegram_loader` - Дополнительно запустит загрузчик из телеграм
 - `docker-compose -f docker-compose-dev.yml up --build --force-recreate s3 postgres telegram_auth elasticsearch dotnet_backend` - Запустит все для локальной разработки frontend части
   - `docker-compose -f docker-compose-dev.yml up --build --force-recreate s3 postgres telegram_auth elasticsearch dotnet_backend telegram_loader` - Дополнительно запустит загрузчик из телеграм
   

## Боевой запуск
`docker-compose up` и после этого ввести код из telegram в отдельную таблицу в PostreSQL

### Данные для подключения локально:

#### S3
    host: localhost
    port: 4566

#### PostgreSQL
    host: localhost
    port: 5432
    user: FileStorageApp
    dbName: FileStorageApp
    password: change

#### ElasticSearch
    host: localhost
    port: 9200

