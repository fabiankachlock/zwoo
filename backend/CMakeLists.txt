cmake_minimum_required(VERSION 3.0)
project(zwoo_backend)

set(CMAKE_CXX_STANDARD 11)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_BINARY_DIR})

find_package(spdlog REQUIRED)
# find_package(oatpp          1.3.0 REQUIRED)
# find_package(oatpp-mongo    1.3.0 REQUIRED)
# find_package(mongocxx             REQUIRED)

option(BUILD_SWAGGER "Activate the build of Swagger Components" OFF)

set(SRC_FILES
    "src/main.cpp"
)
#    "src/zwoo.cpp"
#    "src/HttpServer.cpp"
#    "src/Server/ErrorHandler.cpp"
#)

if(${BUILD_SWAGGER})
	add_definitions(-DBUILD_SWAGGER=1 -DOATPP_SWAGGER_RES_PATH="${oatpp-swagger_INCLUDE_DIRS}/../bin/oatpp-swagger/res")
endif(${BUILD_SWAGGER})

include_directories("include/")

add_executable(main ${SRC_FILES})
target_link_libraries(main spdlog::spdlog)
