FROM node:14-alpine as builder

WORKDIR /frontend

COPY package.json ./
COPY yarn.lock ./

RUN yarn
COPY . .
RUN yarn build

# production stage
FROM nginx:stable-alpine as prod
COPY --from=builder /frontend/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/


CMD ["nginx", "-g", "daemon off;"]