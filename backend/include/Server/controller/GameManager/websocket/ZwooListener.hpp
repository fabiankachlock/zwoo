#ifndef _ZWOOLISTENER_HPP_
#define _ZWOOLISTENER_HPP_

#include "oatpp-websocket/ConnectionHandler.hpp"
#include "oatpp-websocket/WebSocket.hpp"

#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"
#include "Server/logger/logger.h"

#include <unordered_map>
#include <functional>
#include <string>

class ZwooListener : public oatpp::websocket::WebSocket::Listener {
public:

    ZwooListener(std::shared_ptr<Logger> _logger, std::shared_ptr<ZRPConnector> conn) : logger(_logger), connector(conn) {}

    /**
    * Called on "ping" frame.
    */
    void onPing(const WebSocket& socket, const oatpp::String& message) override;

    /**
     * Called on "pong" frame
     */
    void onPong(const WebSocket& socket, const oatpp::String& message) override;

    /**
     * Called on "close" frame
     */
    void onClose(const WebSocket& socket, v_uint16 code, const oatpp::String& message) override;

    /**
     * Called on each message frame. After the last message will be called once-again with size == 0 to designate end of the message.
     */
    void readMessage(const WebSocket& socket, v_uint8 opcode, p_char8 data, oatpp::v_io_size size) override;

private:
    std::shared_ptr<Logger> logger;
    std::shared_ptr<ZRPConnector> connector;

    oatpp::data::stream::BufferOutputStream m_messageBuffer;
};

#endif // _ZWOOLISTENER_HPP_
