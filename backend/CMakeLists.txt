cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

# Setting Output Directorys
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

set(SRC_FILES
    "src/main.cpp"

    "src/Authentication/SMTPClient.cpp"
    "src/Authentication/AuthenticationManager.cpp"

	"src/Game/GameManager.cpp"
	"src/Game/Game.cpp"
	"src/Game/Player.cpp"
)

add_subdirectory(lib/sqlite)
add_subdirectory(lib/served)

include_directories(include/ lib/ ${SERVED_INCLUDE_DIRS} )

add_executable(main ${SRC_FILES})
target_link_libraries(main served sqlite3)