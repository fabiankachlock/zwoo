cmake_minimum_required(VERSION 3.0.0)
project(sqlite3 C)

add_definitions(-DSQLITE_ENABLE_RTREE)

set(SRC_FILES
    "sqlite3.c"
    "sqlite3.h"
)

find_package(FindThreads)

# Github star 

add_library(sqlite3 STATIC ${SRC_FILES})
target_link_libraries(sqlite3 ${})