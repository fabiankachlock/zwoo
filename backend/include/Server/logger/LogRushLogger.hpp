#ifndef _LOGRUSHLOGGER_HPP_
#define _LOGRUSHLOGGER_HPP_

#include <logrush/logrush.h>

#include <spdlog/details/fmt_helper.h>
#include <spdlog/details/os.h>
#include <spdlog/pattern_formatter.h>
#include <spdlog/sinks/sink.h>

class LogRushBasicSink : public spdlog::sinks::sink
{
  public:
    LogRushBasicSink( std::string url, std::string alias, std::string id,
                      std::string key,
                      spdlog::color_mode mode = spdlog::color_mode::never )
        : client( logrush::LogRushClient( url ) ),
          log_stream( client.register_stream( alias, id, key ) ),
          mutex_( std::mutex( ) ),
          formatter_(
              spdlog::details::make_unique<spdlog::pattern_formatter>( ) )
    {
    }
    LogRushBasicSink( const LogRushBasicSink &other ) = delete;
    ~LogRushBasicSink( ) { client.unregister_stream( log_stream ); }

    void log( const spdlog::details::log_msg &msg ) override
    {
        std::lock_guard<std::mutex> lock( mutex_ );
        spdlog::memory_buf_t formatted;
        formatter_->format( msg, formatted );
        log_stream.log( std::string(
            formatted.begin( ),
            std::distance( formatted.begin( ), formatted.end( ) ) ) );
    }

    void flush( ) {}
    void set_pattern( const std::string &pattern ) final
    {
        const std::lock_guard<std::mutex> lock{ mutex_ };
        formatter_ = std::make_unique<spdlog::pattern_formatter>( pattern );
    }
    void
    set_formatter( std::unique_ptr<spdlog::formatter> sink_formatter ) override
    {
        const std::lock_guard<std::mutex> lock{ mutex_ };
        formatter_ = std::move( sink_formatter );
    }

  private:
    logrush::LogRushClient client;
    logrush::BasicLogStream log_stream;

    std::mutex mutex_;
    std::unique_ptr<spdlog::formatter> formatter_;

  public:
    LogRushBasicSink &operator=( const LogRushBasicSink &other ) = delete;
};

#endif