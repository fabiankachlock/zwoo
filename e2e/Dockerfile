FROM docker:20.10.21-dind

# copy everithing into the container
WORKDIR /app
COPY ./ ./

# make script executeable
RUN chmod +x /app/e2e/run-tests.sh

# run scripts on start
ENTRYPOINT /app/e2e/run-tests.sh
