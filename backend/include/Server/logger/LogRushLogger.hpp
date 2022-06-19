#ifndef _LOGRUSHLOGGER_HPP_
#define _LOGRUSHLOGGER_HPP_

#include <mutex>

#include <logrush/logrush.h>

#include <fmt/color.h>
#include <spdlog/details/fmt_helper.h>
#include <spdlog/details/os.h>
#include <spdlog/pattern_formatter.h>
#include <spdlog/sinks/sink.h>

class LogRushBasicSink : public spdlog::sinks::sink
{
  public:
    LogRushBasicSink( std::string url, std::string alias, std::string id,
                      std::string key,
                      spdlog::color_mode mode = spdlog::color_mode::automatic )
        : client( logrush::LogRushClient( url ) ),
          log_stream( client.register_stream( alias, id, key ) ),
          mutex_( std::mutex( ) ),
          formatter_(
              spdlog::details::make_unique<spdlog::pattern_formatter>( ) )
    {
        styles_[ spdlog::level::trace ] =
            fmt::fg( fmt::terminal_color::bright_cyan ) | fmt::emphasis::bold;
        styles_[ spdlog::level::debug ] =
            fmt::fg( fmt::terminal_color::bright_white ) | fmt::emphasis::bold;
        styles_[ spdlog::level::info ] =
            fmt::fg( fmt::terminal_color::bright_green ) | fmt::emphasis::bold;
        styles_[ spdlog::level::warn ] =
            fmt::fg( fmt::terminal_color::bright_yellow ) | fmt::emphasis::bold;
        styles_[ spdlog::level::err ] =
            fmt::fg( fmt::terminal_color::bright_red ) | fmt::emphasis::bold;
        styles_[ spdlog::level::critical ] =
            fmt::fg( fmt::terminal_color::bright_magenta ) |
            fmt::emphasis::bold;
        set_color_mode( mode );
    }
    LogRushBasicSink( const LogRushBasicSink &other ) = delete;
    ~LogRushBasicSink( ) { client.unregister_stream( log_stream ); }

    void log( const spdlog::details::log_msg &msg ) override
    {
        std::lock_guard<std::mutex> lock( mutex_ );
        spdlog::memory_buf_t formatted;
        formatter_->format( msg, formatted );

        auto s = std::string( formatted.begin( ),
                              formatted.begin( ) + msg.color_range_start );
        auto c = fmt::format(
            styles_[ msg.level ],
            std::string_view{ formatted.data( ) + msg.color_range_start,
                              msg.color_range_end - msg.color_range_start } );
        auto e = std::string( formatted.begin( ) + msg.color_range_end,
                              formatted.end( ) );

        log_stream.log( s + c + e );
    }

    void set_color_mode( spdlog::color_mode mode )
    {
        std::lock_guard lock{ mutex_ };
        switch ( mode )
        {
        case spdlog::color_mode::always:
            should_color_ = true;
            break;
        case spdlog::color_mode::automatic:
            should_color_ = true;
            break;
        case spdlog::color_mode::never:
            should_color_ = false;
            break;
        }
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
    std::array<fmt::text_style, spdlog::level::off> styles_;
    bool should_color_ = false;

  public:
    LogRushBasicSink &operator=( const LogRushBasicSink &other ) = delete;
};

#endif