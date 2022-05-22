#include "Server/logger/logger.h"

#include "Server/logger/LogRushLogger.hpp"
#include "spdlog/sinks/daily_file_sink.h"

#include "zwoo.h"

#include <chrono>
#include <iomanip>
#include <iostream>
#include <spdlog/sinks/stdout_color_sinks.h>
#include <sstream>
#include <string>
#include <vector>

void Logger::init( std::string name )
{
    std::vector<spdlog::sink_ptr> log_sinks;
    if ( ZWOO_USE_LOGRUSH )
        log_sinks.emplace_back( std::make_shared<LogRushBasicSink>(
            ZWOO_LOGRUSH_URL, ZWOO_LOGRUSH_ALIAS, ZWOO_LOGRUSH_ID,
            ZWOO_LOGRUSH_KEY ) );
    log_sinks.emplace_back(
        std::make_shared<spdlog::sinks::stdout_color_sink_mt>( ) );
    log_sinks.emplace_back(
        std::make_shared<spdlog::sinks::daily_file_format_sink_mt>(
            ZWOO_LOGS + "%d-%m-%Y_%H:%M:%S-log.txt", 23, 59, false, 3 ) );

    log = std::make_shared<spdlog::logger>( name, begin( log_sinks ),
                                            end( log_sinks ) );
    spdlog::register_logger( log );
    log->set_level( spdlog::level::debug );
    log->flush_on( spdlog::level::debug );
}
