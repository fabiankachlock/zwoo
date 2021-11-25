# GCC support can be specified at major, minor, or micro version
# (e.g. 8, 8.2 or 8.2.0).
# See https://hub.docker.com/r/library/gcc/ for all supported GCC
# tags from Docker Hub.
# See https://docs.docker.com/samples/library/gcc/ for more on how to use this image
FROM ubuntu:latest

WORKDIR /backend

# These commands copy your files into the specified directory in the image
# and set that as the working location
COPY . /backend

RUN ./setup.sh

# This command runs your application, comment out this line to compile only
CMD ["./bin/main"]

LABEL Name=backend Version=0.0.1
