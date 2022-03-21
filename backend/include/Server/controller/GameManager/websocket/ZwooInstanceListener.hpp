#ifndef _ZWOOINSTANCELISTENER_HPP_
#define _ZWOOINSTANCELISTENER_HPP_

#include "oatpp-websocket/ConnectionHandler.hpp"
#include "oatpp-websocket/WebSocket.hpp"

#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"
#include "Server/logger/logger.h"

class ZwooInstanceListener : public oatpp::websocket::ConnectionHandler::SocketInstanceListener {
public:
    ZwooInstanceListener(std::shared_ptr<Logger> _log, std::shared_ptr<ZRPConnector> connector) : logger(_log), connector(connector) {}

    void onAfterCreate(const oatpp::websocket::WebSocket& socket, const std::shared_ptr<const ParameterMap>& params) override;
    void onBeforeDestroy(const oatpp::websocket::WebSocket& socket) override;

private:
    std::shared_ptr<Logger> logger;
    std::shared_ptr<ZRPConnector> connector;

    static std::atomic<v_int32> SOCKETS;
};

#endif // _ZWOOINSTANCELISTENER_HPP_
