#include "Server/controller/GameManager/websocket/ZwooInstanceListener.hpp"

#include "Server/controller/GameManager/websocket/ZwooListener.hpp"

#include <iostream>

std::atomic<v_int32> ZwooInstanceListener::SOCKETS(0);

void ZwooInstanceListener::setLogger(std::shared_ptr<Logger> _logger) { logger = _logger; }
void ZwooInstanceListener::setConnector(std::shared_ptr<ZRPConnector> _connector) { connector = _connector; }

void ZwooInstanceListener::onAfterCreate(const oatpp::websocket::WebSocket& socket, const std::shared_ptr<const ParameterMap>& params)
{
    SOCKETS++;
    logger->log->info("On After Create");
    auto l = std::make_shared<ZwooListener>(logger, connector);

    socket.setListener(l);
}

void ZwooInstanceListener::onBeforeDestroy(const oatpp::websocket::WebSocket& socket)
{
    SOCKETS--;
    logger->log->info("On Before Destroye");
}
