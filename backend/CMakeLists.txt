cmake_minimum_required(VERSION 3.0)
project(zwoo_backend)

set(CMAKE_CXX_STANDARD 11)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_BINARY_DIR})

find_package(spdlog               REQUIRED)
find_package(oatpp          1.3.0 REQUIRED)
# find_package(oatpp-mongo    1.3.0 REQUIRED)
# find_package(mongocxx             REQUIRED)

option(BUILD_SWAGGER "Activate the build of Swagger Components" ON)

set(SRC_FILES
    "src/main.cpp"
    "src/zwoo.cpp"
    "src/HttpServer.cpp"

    "src/Server/ErrorHandler.cpp"
    "src/Server/logger/logger.cpp"
)

set(LIBS
    spdlog::spdlog
    oatpp::oatpp
)

add_executable(main ${SRC_FILES})

if(${BUILD_SWAGGER})
    find_package(oatpp-swagger  1.3.0 REQUIRED)
    add_custom_command(TARGET main POST_BUILD
                       COMMAND ${CMAKE_COMMAND} -E copy_directory
                            ${oatpp_oatpp-swagger_INCLUDE_DIRS}/../bin/oatpp-swagger/res ${CMAKE_BINARY_DIR}/res
    )
	add_definitions(-DBUILD_SWAGGER=1 -DOATPP_SWAGGER_RES_PATH="./res")
	set(LIBS ${LIBS} oatpp::oatpp-swagger)
endif(${BUILD_SWAGGER})

include_directories("include/")
target_link_libraries(main ${LIBS})
