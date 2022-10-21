#!/bin/sh

apk update
apk add mongodb-tools

echo "Importing users..."
echo "USer data:"
cat /app/data/users.json

mongoimport \
    --uri mongodb://db:27017/zwoo \
    --collection users \
    --type json --jsonArray \
    --file /app/data/users.json