FROM node:14 as build
ARG Develop
WORKDIR /app
COPY package.json /app/package.json
COPY . /app
RUN yarn install --production

RUN rm -rf /app/.env

RUN cp .env.config.dev /app/.env;

RUN yarn build

# Поднимаен нгинкс для статики прода
FROM nginx:1.21 AS start_prod
COPY --from=build /app/build /usr/share/nginx/html
COPY ./.nginx/nginx.conf /etc/nginx/nginx.conf

CMD ["nginx", "-g", "daemon off;"]