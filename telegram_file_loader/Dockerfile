FROM python:3.9

ARG Test="false"


RUN mkdir -m 777 -p /app
WORKDIR /app

COPY requirements.txt requirements.txt
COPY requirements-dev.txt requirements-dev.txt

RUN if [ "$Test" == "true" ]; then \
        pip3 install \
        --requirement /app/requirements-dev.txt; \
    else \
        pip3 install \
        --requirement /app/requirements.txt; \
    fi;

COPY . .

RUN if [ "$Test" == "true" ]; then \
        cp  config/local.py; \
    fi;

ENTRYPOINT python3 /app/server.py