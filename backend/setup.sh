#!/bin/sh
apt update && DEBIAN_FRONTEND="noninteractive" TZ="Germany/Berlin" apt-get install -y tzdata
apt update
apt install -y build-essential cmake protobuf-compiler libssl-dev libcurl4-openssl-dev libsasl2-dev zlib1g-dev git

git clone https://github.com/oatpp/oatpp.git
cd oatpp

mkdir build && cd build
cmake ..
make install -j$(nproc)

cd ../..
rm oatpp -rdf

git clone https://github.com/oatpp/oatpp-swagger.git
cd oatpp-swagger

mkdir build && cd build
cmake ..
make install -j$(nproc)

cd ../..
rm oatpp-swagger -rdf


#cd backend
cmake . && make -j$(nproc)
