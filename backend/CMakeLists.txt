cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

find_package(Boost REQUIRED)

if (UNIX)
    if (NOT APPLE)
        find_package(Threads REQUIRED)
    endif()
endif()

# Setting Output Directorys
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

set (CMAKE_CXX_STANDARD 17)

set(SRC_FILES
    "src/main.cpp"

    "src/Authentication/SMTPClient.cpp"
    "src/Authentication/AuthenticationManager.cpp"

	"src/Game/GameManager.cpp"
	"src/Game/Game.cpp"
	"src/Game/Player.cpp"
)

set(SERVED_SRC 
	"lib/served/methods_handler.cpp"
	"lib/served/multiplexer.cpp"
	"lib/served/parameters.cpp"
	"lib/served/request_parser_impl.cpp"
	"lib/served/request_parser.cpp"
	"lib/served/request.cpp"
	"lib/served/response.cpp"
	"lib/served/status.cpp"
	"lib/served/uri.cpp"

	"lib/served/mux/regex_matcher.cpp"
	"lib/served/mux/static_matcher.cpp"
	"lib/served/mux/variable_matcher.cpp"

	"lib/served/net/connection_manager.cpp"
	"lib/served/net/connection.cpp"
	"lib/served/net/server.cpp"
)

add_subdirectory(lib/sqlite)

find_package(CURL REQUIRED)

message("Libs: " ${CMAKE_DL_LIBS})

include_directories(include/ lib/ ${CURL_INCLUDE_DIRS} ${Boost_INCLUDE_DIRS} )

add_executable(main ${SERVED_SRC} ${SQLITE_SRC} ${SRC_FILES})
target_link_libraries(main ${CURL_LIBRARIES} ${CMAKE_THREAD_LIBS_INIT} ${CMAKE_DL_LIBS} ${Boost_LIBRARIES} sqlite3)