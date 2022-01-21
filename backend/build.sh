#!/bin/sh

echo username: $1
echo password: $2

apt update
apt update && DEBIAN_FRONTEND="noninteractive" TZ="Germany/Berlin" apt-get install -y tzdata
apt install -y build-essential cmake python3 python3-pip
pip install conan

conan remote add conan-remote $3
conan user -p $2 -r conan-remote $1

mkdir build && cd build
conan install .. --build
cmake ..

make -j$(nproc)
