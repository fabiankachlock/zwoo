#!/bin/sh

# assume all test are failing
CY_KEY=$1
EL_EXIT_CODE=1
CH_EXIT_CODE=1
FI_EXIT_CODE=1
ED_EXIT_CODE=1

launch_app() {
    # start env
    echo ""
    echo "starting application..."
    docker compose -f ./e2e/docker-compose.e2e.yml up -d --wait
    echo "application started!"
}

shutdown_app() {
    # shot down env
    echo "shutting down application..."
    docker compose -f ./e2e/docker-compose.e2e.yml down
    echo "application shut down!."
    echo ""
}

mkdir -p /app/frontend/uploads/screenshots
mkdir -p /app/frontend/uploads/videos

echo "waiting for docker..."
# wait for docker
until docker ps > /dev/null 2>&1
do
  sleep 1
done
echo "docker ready!"

echo "===== COMMIT INFO ====="
cat cypress.env

# run tests
# echo ""
# echo "===== ELECTRON ====="
# echo ""
# launch_app
# echo "running tests in electron..."
# if [ -n "$CY_KEY" ]; then
#     docker run -i --name e2e-test-runner-electron --rm -v /app/frontend:/e2e -w /e2e --network host cypress/included:13.5.0 --record --key $CY_KEY
# else
#     # dont record when the key is not supplied
#     docker run -i --name e2e-test-runner-electron --rm -v /app/frontend:/e2e -w /e2e --network host cypress/included:13.5.0
# fi
# EL_EXIT_CODE=$?
# echo "electron tests ran!"
# shutdown_app

echo ""
echo "===== CHROME ====="
echo ""
launch_app
echo "running tests in chrome..."
if [ -n "$CY_KEY" ]; then
    docker run -i --name e2e-test-runner-chrome --rm -v /app/frontend:/e2e -w /e2e --env-file ./cypress.env --network host cypress/included:13.5.0 --browser chrome --record --key $CY_KEY
else
    # dont record when the key is not supplied
    docker run -i --name e2e-test-runner-chrome --rm -v /app/frontend:/e2e -w /e2e --env-file ./cypress.env --network host cypress/included:13.5.0 --browser chrome
fi
CH_EXIT_CODE=$?
cp /app/frontend/tests/e2e/screenshots /app/frontend/uploads/screenshots/chrome
cp /app/frontend/tests/e2e/videos /app/frontend/uploads/videos/chrome
echo "chrome tests ran!"
shutdown_app

echo ""
echo "===== FIREFOX ====="
echo ""
launch_app
echo "running tests in firefox..."
if [ -n "$CY_KEY" ]; then
    docker run -i --name e2e-test-runner-firefox --rm -v /app/frontend:/e2e -w /e2e --env-file ./cypress.env --network host cypress/included:13.5.0 --browser firefox --record --key $CY_KEY
else
    # dont record when the key is not supplied
    docker run -i --name e2e-test-runner-firefox --rm -v /app/frontend:/e2e -w /e2e --env-file ./cypress.env --network host cypress/included:13.5.0 --browser firefox
fi
FI_EXIT_CODE=$?
cp /app/frontend/tests/e2e/screenshots /app/frontend/uploads/screenshots/firefox
cp /app/frontend/tests/e2e/videos /app/frontend/uploads/videos/firefox
echo "firefox tests ran!"
shutdown_app

echo ""
echo "===== EDGE ====="
echo ""
launch_app
echo "running tests in edge..."
if [ -n "$CY_KEY" ]; then
    docker run -i --name e2e-test-runner-edge --rm -v /app/frontend:/e2e -w /e2e --env-file ./cypress.env --network host cypress/included:13.5.0 --browser edge --record --key $CY_KEY
else
    # dont record when the key is not supplied
    docker run -i --name e2e-test-runner-edge --rm -v /app/frontend:/e2e -w /e2e --env-file ./cypress.env --network host cypress/included:13.5.0 --browser edge
fi
ED_EXIT_CODE=$?
cp /app/frontend/tests/e2e/screenshots /app/frontend/uploads/screenshots/egde
cp /app/frontend/tests/e2e/videos /app/frontend/uploads/videos/egde
echo "edge tests ran!"
shutdown_app

echo ""
echo "===== SUMMARY ====="
echo ""

EXIT_CODE=0
# print status
# if [ $EL_EXIT_CODE != 0 ]; then
#     echo "ERR electron tests failed!"
# else
#     echo "SUCCESS electron tests ran successfully!"
# fi

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