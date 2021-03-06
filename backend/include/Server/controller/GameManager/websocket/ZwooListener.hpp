#ifndef _ZWOOLISTENER_HPP_
#define _ZWOOLISTENER_HPP_

#include "Server/logger/logger.h"
#include "oatpp-websocket/ConnectionHandler.hpp"
#include "oatpp-websocket/WebSocket.hpp"

#include <functional>
#include <string>
#include <unordered_map>

class ZRPConnector;

struct ListenerData
{
    uint32_t guid;
    uint32_t puid;
    uint8_t role;
    oatpp::String username;
    uint32_t wins;
    uint8_t role_next_round = role;
};

class ZwooListener : public oatpp::websocket::WebSocket::Listener
{
  public:
    ZwooListener( std::shared_ptr<Logger> _logger,
                  std::shared_ptr<ZRPConnector> conn, ListenerData _data,
                  const WebSocket &socket )
        : logger( _logger ), connector( conn ), m_data( _data ),
          websocket( socket )
    {
    }

  private:
    std::shared_ptr<Logger> logger;
    std::shared_ptr<ZRPConnector> connector;

    oatpp::data::stream::BufferOutputStream m_messageBuffer;

  public:
    ListenerData m_data;
    const WebSocket &websocket;

    /**
     * Called on "ping" frame.
     */
    void onPing( const WebSocket &socket,
                 const oatpp::String &message ) override;

    /**
     * Called on "pong" frame
     */
    void onPong( const WebSocket &socket,
                 const oatpp::String &message ) override;

    /**
     * Called on "close" frame
     */
    void onClose( const WebSocket &socket, v_uint16 code,
                  const oatpp::String &message ) override;

    /**
     * Called on each message frame. After the last message will be called
     * once-again with size == 0 to designate end of the message.
     */
    void readMessage( const WebSocket &socket, v_uint8 opcode, p_char8 data,
                      oatpp::v_io_size size ) override;
};

#endif // _ZWOOLISTENER_HPP_
