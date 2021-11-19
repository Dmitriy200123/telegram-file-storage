FROM node:12-alpine as builder
WORKDIR /app
COPY . /app

RUN npm install 

CMD ["npm", "run", "start"]
