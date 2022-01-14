cmake_minimum_required(VERSION 3.0)
project(zwoo_backend)

set(SRC_FILES
    "src/main.cpp"
)

option(BUILD_SWAGGER "Activate the build of Swagger Components" ON)

include(${CMAKE_BINARY_DIR}/conanbuildinfo.cmake)
conan_basic_setup()

if(${BUILD_SWAGGER})
	add_definitions(-DBUILD_SWAGGER=1 -DOATPP_SWAGGER_RES_PATH="${oatpp-swagger_INCLUDE_DIRS}/../bin/oatpp-swagger/res")
endif(${BUILD_SWAGGER})

add_executable(main ${SRC_FILES})
target_link_libraries(main ${CONAN_LIBS})