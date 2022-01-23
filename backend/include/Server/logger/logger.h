#ifndef _LOGGER_H_
#define _LOGGER_H_

#include <string>
#include <vector>
#include <sstream>
#include <chrono>
#include <iomanip>

#include <spdlog/spdlog.h>

class Logger {
public:
    Logger() {};

    void init();

    std::shared_ptr<spdlog::logger> log;
};

#endif
