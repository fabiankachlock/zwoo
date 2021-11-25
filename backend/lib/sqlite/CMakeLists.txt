cmake_minimum_required(VERSION 3.0.0)
project(sqlite3 C)

# Setting Output Directorys
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

add_definitions(-DSQLITE_ENABLE_RTREE)
add_definitions(-DSQLITE_ENABLE_FTS4)
add_definitions(-DSQLITE_ENABLE_FTS5)
add_definitions(-DSQLITE_ENABLE_JSON1)
add_definitions(-DSQLITE_ENABLE_RBU)
add_definitions(-DSQLITE_ENABLE_STAT4)

set(SRC_FILES
    "sqlite3.c"
    "sqlite3.h"
    "sqlite3ext.h"
)

find_package(Threads REQUIRED)

# Github star 

add_library(sqlite3 STATIC ${SRC_FILES})
target_link_libraries(sqlite3 ${CMAKE_THREAD_LIBS_INIT} ${CMAKE_DL_LIBS})