# Telegram file storage

## Локальная разработка
В корне проекта (/FileStorageApp) находится `docker-compose` файл, описывающий развертку всех необходимых сервисов для запуска на локальной машине 

Запуск конейнеров aws s3 и postgreSQL:` docker-compose up --build --force-recreate postgres s3`

