#include "Server/logger/logger.h"

#include <iostream>
#include <sstream>
#include <string>
#include <chrono>
#include <iomanip>
#include <vector>

#include <spdlog/sinks/stdout_color_sinks.h>
#include <spdlog/sinks/basic_file_sink.h>

void Logger::init(std::string name)
{
    std::stringstream string_stream;
    auto timer = std::time(NULL);
    auto tm = *std::localtime(&timer);
    string_stream << std::put_time(&tm, fmt::format("%d-%m-%Y_%H:%M:%S-log.txt").c_str());

    std::vector<spdlog::sink_ptr> log_sinks;
    log_sinks.emplace_back(std::make_shared<spdlog::sinks::stdout_color_sink_mt>());
    log_sinks.emplace_back(std::make_shared<spdlog::sinks::basic_file_sink_mt>(string_stream.str(), true));

    log = std::make_shared<spdlog::logger>(name, begin(log_sinks), end(log_sinks));
    spdlog::register_logger(log);
    log->set_level(spdlog::level::debug);
    log->flush_on(spdlog::level::debug);
}
