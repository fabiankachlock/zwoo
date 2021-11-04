#!/bin/sh
apt-get update && DEBIAN_FRONTEND="noninteractive" TZ="Germany/Berlin" apt-get install -y tzdata
apt-get install -y build-essential cmake libboost-all-dev protobuf-compiler libssl-dev libcurl4-openssl-dev libsasl2-dev zlib1g-dev
 
wget https://dev.mysql.com/get/Downloads/Connector-C++/libmysqlcppconn9_8.0.27-1ubuntu20.04_amd64.deb
dpkg --install ./libmysqlcppconn9_8.0.27-1ubuntu20.04_amd64.deb
rm ./libmysqlcppconn9_8.0.27-1ubuntu20.04_amd64.deb

#cd backend
cmake . && make
