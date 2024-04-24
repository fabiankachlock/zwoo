FROM alpine

RUN apk update
RUN apk add mongodb-tools

COPY ./data /app/data
COPY ./import.sh /app/import.sh

RUN chmod +x /app/import.sh
