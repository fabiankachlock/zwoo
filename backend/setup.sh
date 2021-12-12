#!/bin/sh
install_oatpp_mongo() {
    build_mongo_drivers

    git clone https://github.com/oatpp/oatpp-mongo.git
    cd oatpp-mongo

    mkdir build && cd build
    cmake -DOATPP_BUILD_TESTS=OFF ..
    make install -j$(nproc)

    cd ../..
    rm oatpp-mongo -rdf
}

build_mongo_drivers(){
  TOP_DIR=$(pwd)

  wget https://github.com/mongodb/mongo-c-driver/releases/download/1.20.0/mongo-c-driver-1.20.0.tar.gz
  tar xzf mongo-c-driver-1.20.0.tar.gz
  cd mongo-c-driver-1.20.0
  mkdir cmake-build
  cd cmake-build
  cmake -DENABLE_AUTOMATIC_INIT_AND_CLEANUP=OFF ..
  make install -j$(nproc)

  git clone https://github.com/mongodb/mongo-cxx-driver.git \
      --branch releases/stable --depth 1
  cd mongo-cxx-driver/build

  cmake .. \
      -DCMAKE_BUILD_TYPE=Release \
      -DBSONCXX_POLY_USE_MNMLSTC=1 \
      -DCMAKE_INSTALL_PREFIX=/usr/local

  make -j$(nproc)
  make install
  cd ../../../../
  rm -rf mongo-c-driver-1.20.0*
}

install_oatpp_swagger() {
    git clone https://github.com/oatpp/oatpp-swagger.git
    cd oatpp-swagger

    mkdir build && cd build
    cmake ..
    make install -j$(nproc)

    cd ../..
    rm oatpp-swagger -rdf
}

install_oatpp_base() {
    git clone https://github.com/oatpp/oatpp.git
    cd oatpp

    mkdir build && cd build
    cmake ..
    make install -j$(nproc)

    cd ../..
    rm oatpp -rdf
}

install_oatpp() {
    install_oatpp_base
    install_oatpp_mongo
    install_oatpp_swagger
}

get_build_deps() {
    apt update && DEBIAN_FRONTEND="noninteractive" TZ="Germany/Berlin" apt-get install -y tzdata
    apt update
    apt install -y build-essential cmake protobuf-compiler git wget pkg-config python3
    apt install -y libevent-openssl-2.1-7 libmongoc-1.0-0 libbson-1.0-0 libbson-dev libssl-dev libcrypto-dev libcurl4-openssl-dev libsasl2-dev zlib1g-dev
}

build_zwoo_backend() {
    cmake . && make -j$(nproc)
}

cleanup() {
    rm CMakeFiles -rdf
    rm src -rdf
    rm lib -rdf
    rm include -rdf
    apt remove -y python3 libbson-dev build-essential cmake protobuf-compiler libssl-dev libcurl4-openssl-dev libsasl2-dev zlib1g-dev git
    apt autoremove -y 
}

main() {
    get_build_deps
    install_oatpp

    pkg-config --list-all | grep curl

    cmake . && make -j$(nproc)

    cleanup
}

main