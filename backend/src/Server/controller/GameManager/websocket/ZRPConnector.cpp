#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"

#include "Server/dto/ZRPMessageDTO.hpp"

ZRPConnector::ZRPConnector()
{
    logger = std::make_shared<Logger>();
    logger->init("ZRPConnector");

    json_mapper = oatpp::parser::json::mapping::ObjectMapper::createShared();
}

void ZRPConnector::addWebSocket(uint32_t guid, uint32_t puid, std::shared_ptr<ZwooListener> listener)
{
    // TODO Player/Spectator Joined (100, 101)
    auto game = game_websockets.find(guid);
    if (game != game_websockets.end())
        game->second[puid] = listener;
    else
        game_websockets[guid] = { { puid, listener } };
    printWebsockets();
}

void ZRPConnector::removeWebSocket(uint32_t guid, uint32_t puid)
{
    // TODO Player/Spectator Joined (102, 103)
    auto game = game_websockets.find(guid);
    if (game != game_websockets.end())
    {
        auto peer = game->second.find(puid);
        if (peer != game->second.end())
        {
            game->second.erase(peer);
            if (game->second.size() == 0)
                game_websockets.erase(game);
            printWebsockets();
        }
        else
            logger->log->critical("No peer with ID {0} found can not remove WebSocket!", puid);
    }
    else
        logger->log->critical("No game with GameID {0} found can not remove WebSocket!", guid);
}


void ZRPConnector::sendMessage(uint32_t guid, uint32_t puid, std::string data)
{
    auto send_message = json_mapper->readFromString<oatpp::Object<SendMessage>>(removeZRPCode(data));
    auto sender = getSocket(guid, puid);
    auto game = game_websockets.find(guid);

    if (sender != nullptr && game != game_websockets.end())
    {
        auto message = ReceiveMessage::createShared();
        message->message = send_message->message;
        message->name = sender->m_data.username;
        message->role = sender->m_data.role;

        auto out = createMessage(105, json_mapper->writeToString(message));

        for (const auto&[k, v] : game->second)
            v->websocket.sendOneFrameText(out);
    }
    else
        logger->log->error("No Player with ID: {} or game with ID: {} found!", puid, guid);
    logger->log->info("guid: {0}, puid: {1}, message: {2}", guid, puid, send_message->message->c_str());
}

void ZRPConnector::printWebsockets()
{
    for (const auto&[k1, v1] : game_websockets)
    {
        logger->log->info("{0}:", k1);
        for (const auto&[k2, v2] : v1)
            logger->log->info("  {0}: {1},", k2, v2->m_data.username.getValue(""));
    }
}

std::string ZRPConnector::removeZRPCode(std::string data)
{
    return data.substr(4, data.size() - 4);
}

std::string ZRPConnector::createMessage(int code, std::string data)
{
    return std::to_string(code) + "," + data;
}

std::shared_ptr<ZwooListener> ZRPConnector::getSocket(uint32_t guid, uint32_t puid)
{
    auto game = game_websockets.find(guid);
    if (game != game_websockets.end())
    {
        auto socket = game->second.find(puid);
        if (socket != game->second.end())
            return socket->second;
    }
    return nullptr;
}
