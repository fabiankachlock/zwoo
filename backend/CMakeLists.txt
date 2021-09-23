cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

# Setting Output Directorys
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

include_directories(include/ ${LIBWEBSOCKETS_INCLUDE_DIRS})

find_package(libwebsockets REQUIRED)

set(SRC_FILES
    "src/main.cpp"

    "src/Authentication/SMTPClass.cpp"
)

add_executable(main ${SRC_FILES})
target_link_libraries(main ${LIBWEBSOCKETS_LIBRARIES})