FROM node:12-alpine as build
ARG Develop
WORKDIR /app
COPY package.json /app/package.json
RUN npm install --only=prod
COPY . /app

RUN rm -rf /app/.env

RUN if [ "$Develop" == "true" ]; then \
        cp .env.config.dev /app/.env; \
    else \
        cp .env.config.prod /app/.env; \
    fi;

RUN npm run build

# Поднимаен нгинкс для статики прода
FROM nginx:1.21 AS start_prod
COPY --from=build /app/build /usr/share/nginx/html
COPY ./.nginx/nginx.conf /etc/nginx/nginx.conf

CMD ["nginx", "-g", "daemon off;"]