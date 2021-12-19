# TODO: Cleanup
cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

# Setting Output Directory
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

option(BUILD_SWAGGER "build with Swagger" ON)

find_package(oatpp          1.3.0 REQUIRED)
find_package(oatpp-mongo    1.3.0 REQUIRED)
find_package(mongocxx             REQUIRED)
find_package(CURL                 REQUIRED)

set(SRC_FILES
    "src/main.cpp"

    "src/Authentication/SMTPClient.cpp"
    "src/Authentication/AuthenticationManager.cpp"
	"src/Authentication/reCaptcha.cpp"

	"src/Game/GameManager.cpp"
	"src/Game/Game.cpp"
	"src/Game/Player.cpp"

	"include/Server/ErrorHandler.cpp"
)

set(LIBS
	oatpp::oatpp 
	oatpp::oatpp-mongo
	CURL::libcurl
	mongo::mongocxx_shared
	${CMAKE_THREAD_LIBS_INIT}
)

include_directories(include/ lib/)

add_executable(main ${SRC_FILES})

if(${BUILD_SWAGGER})
	find_package(oatpp-swagger  1.3.0 REQUIRED)
	add_definitions(-DBUILD_SWAGGER=1 -DOATPP_SWAGGER_RES_PATH="${oatpp-swagger_INCLUDE_DIRS}/../bin/oatpp-swagger/res")
	set(LIBS ${LIBS} oatpp::oatpp-swagger)
endif(${BUILD_SWAGGER})

if(CMAKE_SYSTEM_NAME MATCHES Linux)
    find_package(Threads REQUIRED)
	set(LIBS ${LIBS} ${CMAKE_THREAD_LIBS_INIT} ${CMAKE_DL_LIBS})
endif()

target_link_libraries(main ${LIBS})