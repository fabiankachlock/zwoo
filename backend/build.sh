#!/bin/sh

apt update
apt update && DEBIAN_FRONTEND="noninteractive" TZ="Germany/Berlin" apt-get install -y tzdata
apt install -y cmake python3 python3-pip git
pip install conan

conan remote add conan-remote $3
conan user -p $2 -r conan-remote $1

mkdir build && cd build
conan install .. -s compiler.libcxx=libstdc++11 --build
cmake ..

make -j$(nproc)
