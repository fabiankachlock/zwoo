#!/bin/sh
apt-get update && DEBIAN_FRONTEND="noninteractive" TZ="Germany/Berlin" apt-get install -y tzdata
apt-get install -y build-essential cmake libboost-all-dev protobuf-compiler libssl-dev libcurl4-openssl-dev libpqxx-dev
#cd backend 
cmake . && make
