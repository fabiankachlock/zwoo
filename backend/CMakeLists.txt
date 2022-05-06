cmake_minimum_required(VERSION 3.0)
project(zwoo_backend)

set(CMAKE_CXX_STANDARD 17)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_BINARY_DIR})

find_package(spdlog                REQUIRED)
find_package(oatpp           1.3.0 REQUIRED)
find_package(oatpp-mongo     1.3.0 REQUIRED)
find_package(oatpp-websocket 1.3.0 REQUIRED)
find_package(oatpp-openssl   1.3.0 REQUIRED)
find_package(CURL                  REQUIRED)
find_package(mongocxx              REQUIRED)
find_package(mailio                REQUIRED)

option(BUILD_SWAGGER "Activate the build of Swagger Components" ON)
option(BUILD_BETA "Build Zwoo Betacode Application" ON)

set(SRC_FILES
    "src/main.cpp"
    "src/zwoo.cpp"
    "src/HttpServer.cpp"
    "src/SHA512.cpp"
    "src/Helper.cpp"

    "src/utils/thread.cpp"

    "src/Server/ErrorHandler.cpp"
    "src/Server/DatabaseComponent.cpp"
    "src/Server/AuthorizationHandler.cpp"
    "src/Server/ZwooRequestInterceptor.cpp"
    "src/Server/ZwooResponseInterceptor.cpp"
    "src/Server/logger/logger.cpp"

    "src/Server/controller/error.cpp"

    "src/Server/controller/Authentication/ReCaptcha.cpp"
    "src/Server/controller/Authentication/AuthenticationValidators.cpp"

    "src/Server/controller/GameManager/websocket/ZRPConnector.cpp"
    "src/Server/controller/GameManager/websocket/ZwooInstanceListener.cpp"
    "src/Server/controller/GameManager/websocket/ZwooListener.cpp"
)

set(LIBS
    spdlog::spdlog
    oatpp::oatpp
    oatpp::oatpp-mongo
    oatpp::oatpp-websocket
    CURL::libcurl
    mongo::mongocxx_static
    mongo::bsoncxx_static
    mailio::mailio
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

if (${BUILD_BETA})
    add_subdirectory(util/GenerateBETACodes)
endif()

include_directories("include/")
target_link_libraries(main ${LIBS})
