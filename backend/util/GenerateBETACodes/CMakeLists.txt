cmake_minimum_required(VERSION 3.0)
project(zwoo_backend)

set(CMAKE_CXX_STANDARD 17)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_BINARY_DIR})

find_package(mongocxx REQUIRED)
find_package(ftxui REQUIRED)

set(SRC_FILES
    "src/main.cpp"
)

set(LIBS
    ftxui::component
    ftxui::dom
    ftxui::screen
    mongo::mongocxx_static
    mongo::bsoncxx_static
)

add_executable(main ${SRC_FILES})

include_directories("include/")
target_link_libraries(main ${LIBS})
