Проект автозагрузчика файлов(python)

Живем с использованием прекомит хуков


Для это в venv:

`pip install pre-commit`

`pre-commit install -f`

`pre-commit install -f --hook-type pre-push`

Оно должно перед пушем само запускаться, но можно самому руками дернуть через:
`pre-commit run --all-files`