#include "Server/controller/GameManager/websocket/ZwooInstanceListener.hpp"

#include "Server/controller/GameManager/websocket/ZwooListener.hpp"

#include <iostream>

std::atomic<v_int32> ZwooInstanceListener::SOCKETS(0);

void ZwooInstanceListener::onAfterCreate(const oatpp::websocket::WebSocket& socket, const std::shared_ptr<const ParameterMap>& params)
{
    SOCKETS++;

    auto guid = (uint32_t)stoi(params->find("guid")->second.getValue(""));
    auto puid = (uint32_t)stoi(params->find("puid")->second.getValue(""));
    auto role = (uint8_t)stoi(params->find("role")->second.getValue(""));
    auto username = params->find("username")->second.getValue("");

    ListenerData data = { guid, puid, role, username };

    logger->log->info("name: {3}, guid: {0}, puid: {1}, role: {2} Connected!", guid, puid, role, username);

    auto l = std::make_shared<ZwooListener>(logger, connector, data, socket);
    socket.setListener(l);

    connector->addWebSocket(guid, puid, l);
}

void ZwooInstanceListener::onBeforeDestroy(const oatpp::websocket::WebSocket& socket)
{
    SOCKETS--;
    auto listener = std::static_pointer_cast<ZwooListener>(socket.getListener());
    connector->removeWebSocket(listener->m_data.guid, listener->m_data.puid);
}
