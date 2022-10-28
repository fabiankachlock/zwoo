#!/bin/sh

# assume all test are failing
EL_EXIT_CODE=1
CH_EXIT_CODE=1
FI_EXIT_CODE=1
ED_EXIT_CODE=1

# start env
echo "starting application..."
docker compose -f ./docker-compose.e2e.yml up -d --wait
echo "application started!"

# run tests
echo "running tests in electron..."
docker run -i --name e2e-test-runner-electron --rm -v $PWD/e2e/frontend:/e2e -w /e2e --network host cypress/included:10.9.0
EL_EXIT_CODE=$?
echo "electron tests ran!"

echo "running tests in chrome..."
docker run -i --name e2e-test-runner-chrome --rm -v $PWD/e2e/frontend:/e2e -w /e2e --network host cypress/included:10.9.0 --browser chrome
CH_EXIT_CODE=$?
echo "chrome tests ran!"

echo "running tests in firefox..."
docker run -i --name e2e-test-runner-firefox --rm -v $PWD/e2e/frontend:/e2e -w /e2e --network host cypress/included:10.9.0 --browser firefox
FI_EXIT_CODE=$?
echo "firefox tests ran!"

echo "running tests in edge..."
docker run -i --name e2e-test-runner-edge --rm -v $PWD/e2e/frontend:/e2e -w /e2e --network host cypress/included:10.9.0 --browser edge
ED_EXIT_CODE=$?
echo "edge tests ran!"

# shot down env
echo "shutting down application..."
docker compose -f ./docker-compose.e2e.yml down
echo "application shut down!."

EXIT_CODE=0
# print status
if [ $EL_EXIT_CODE != 0 ]; then
    echo "ERR electron tests failed!"
else
    echo "SUCCESS electron tests ran successfully!"
fi

if [ $CH_EXIT_CODE != 0 ]; then
    echo "ERR chrome tests failed!"
    EXIT_CODE=1
else
    echo "SUCCESS chrome tests ran successfully!"
fi

if [ $FI_EXIT_CODE != 0 ]; then
    echo "ERR firefox tests failed!"
    EXIT_CODE=1
else
    echo "SUCCESS firefox tests ran successfully!"
fi

if [ $ED_EXIT_CODE != 0 ]; then
    echo "ERR edge tests failed!"
    EXIT_CODE=1
else
    echo "SUCCESS edge tests ran successfully!"
fi

echo "finsihed test suite!"
exit $EXIT_CODE