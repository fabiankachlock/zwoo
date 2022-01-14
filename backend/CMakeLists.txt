cmake_minimum_required(VERSION 3.0)
project(zwoo_backend)

set(SRC_FILES
    "src/main.cpp"
)

include(${CMAKE_BINARY_DIR}/conanbuildinfo.cmake)
conan_basic_setup()

add_executable(main ${SRC_FILES})
target_link_libraries(main ${CONAN_LIBS})