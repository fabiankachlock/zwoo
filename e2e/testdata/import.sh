#!/bin/sh

BASE_DIR=/app/data
echo "===== zwoo-data-import ====="
echo "searching data files in $BASE_DIR"
FILES=$(ls $BASE_DIR | grep .json)
echo "found:\n$FILES"
echo "starting data import..."

for file in $BASE_DIR/*.json; do
    [ -e "$file" ] || continue
    name=${file##*/}
    base=${name%.json}
    echo "importing data from '$BASE_DIR/$name' into collection '$base'"

    mongoimport \
        --uri mongodb://db:27017/zwoo \
        --collection users \
        --type json --jsonArray \
        --file $BASE_DIR/$name
done

echo "data import finished!"
echo "===== zwoo-data-import ====="

# keep the container running so that docker compose does not wait indefinitely
sleep 60