cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

# Setting Output Directorys
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

set (CMAKE_CXX_STANDARD 17)

set(SRC_FILES
    "src/main.cpp"

    "src/Authentication/SMTPClient.cpp"
    "src/Authentication/Authentication.cpp"
)

find_package(CURL REQUIRED)
find_package(Boost REQUIRED)

include_directories(include/ lib/SimpleJSON lib/served ${Boost_INCLUDE_DIRS} ${CURL_INCLUDE_DIRS})

add_executable(main ${SRC_FILES})
target_link_libraries(main ${CURL_LIBRARIES} ${Boost_LIBRARIES})