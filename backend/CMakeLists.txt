cmake_minimum_required(VERSION 3.0.0)
project(zwoo_backend VERSION 0.1.0)

include(CTest)
enable_testing()

set(SRC_FILES
    "src/main.cpp"

    "src/Authentication/SMTPClass.cpp"
)

add_executable(main ${SRC_FILES})


set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
