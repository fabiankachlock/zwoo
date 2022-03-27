#ifndef _LOGGER_H_
#define _LOGGER_H_

#include <spdlog/spdlog.h>
#include <string>


class Logger {
public:
    Logger() {};

    void init(std::string name);

    std::shared_ptr<spdlog::logger> log;
};

#endif
