cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

# Setting Output Directory
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

option(BUILD_SWAGGER "build with Swagger" ON)

set(SRC_FILES
    "src/main.cpp"
	"src/HttpServer.cpp"

    "src/Authentication/SMTPClient.cpp"
	"src/Authentication/reCaptcha.cpp"

	"src/Database/Database.cpp"

	"src/Game/GameManager.cpp"
	"src/Game/Game.cpp"
	"src/Game/Player.cpp"

	"src/util/SHA512.cpp"
	"src/util/Validator.cpp"
	"src/util/RandomString.cpp"

	"include/Server/ErrorHandler.cpp"
)

### Finde Dependencies ###

find_package(oatpp          1.3.0 REQUIRED)
find_package(oatpp-mongo    1.3.0 REQUIRED)
find_package(mongocxx             REQUIRED)
find_package(CURL                 REQUIRED)

set(LIBS
	oatpp::oatpp 
	oatpp::oatpp-mongo
	CURL::libcurl
	mongo::mongocxx_shared
	${CMAKE_THREAD_LIBS_INIT}
)

if(${BUILD_SWAGGER})
	find_package(oatpp-swagger  1.3.0 REQUIRED)
	add_definitions(-DBUILD_SWAGGER=1 -DOATPP_SWAGGER_RES_PATH="${oatpp-swagger_INCLUDE_DIRS}/../bin/oatpp-swagger/res")
	set(LIBS ${LIBS} oatpp::oatpp-swagger)
endif(${BUILD_SWAGGER})

if(CMAKE_SYSTEM_NAME MATCHES Linux)
    find_package(Threads REQUIRED)
	set(LIBS ${LIBS} ${CMAKE_THREAD_LIBS_INIT} ${CMAKE_DL_LIBS})
endif()

### Build Setup ###

include_directories(include/ lib/)

add_executable(main ${SRC_FILES})

target_link_libraries(main ${LIBS})