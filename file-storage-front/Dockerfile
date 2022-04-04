FROM node:12-alpine as build
WORKDIR /app
COPY package.json /app/package.json
RUN npm install --only=prod
COPY . /app

FROM build AS build_prod
RUN npm run build

# Поднимаен нгинкс для статики прода
FROM nginx:1.16.0-alpine AS start_prod
COPY --from=build_prod /app/build /usr/share/nginx/html
COPY ./.nginx/nginx.conf /etc/nginx/nginx.conf

CMD ["nginx", "-g", "daemon off;"]


#Поднимаем для локальной разработки
FROM build AS start_local

CMD ["npm", "run", "start"]