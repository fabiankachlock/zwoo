#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"

#include "Server/dto/ZRPMessageDTO.hpp"
#include "Server/controller/GameManager/websocket/ZRPCodes.h"

ZRPConnector::ZRPConnector()
{
    logger = std::make_shared<Logger>();
    logger->init("ZRP");

    json_mapper = oatpp::parser::json::mapping::ObjectMapper::createShared();
}

void ZRPConnector::addWebSocket(uint32_t guid, uint32_t puid, std::shared_ptr<ZwooListener> listener)
{
    auto game = game_websockets.find(guid);
    if (game != game_websockets.end())
    {
        if (game->second[puid] != nullptr)
            game->second[puid]->websocket.sendClose(1005, "");
        game->second[puid] = listener;
    }
    else
        game_websockets[guid] = { { puid, listener } };

    printWebsockets();

    game = game_websockets.find(guid);

    {
        auto ps_joined = UserJoined::createShared();
        ps_joined->name = listener->m_data.username;
        ps_joined->wins = listener->m_data.wins;
        ps_joined->role = listener->m_data.role;

        auto out = createMessage((listener->m_data.role == (int)e_Roles::SPECTATOR) ? (int)e_ZRPOpCodes::SPECTATOR_JOINED : (int)e_ZRPOpCodes::PLAYER_JOINED, json_mapper->writeToString(ps_joined));

        for (const auto&[k, v] : game->second)
            if (v != listener)
                v->websocket.sendOneFrameText(out);
    }
}

void ZRPConnector::removeWebSocket(uint32_t guid, uint32_t puid)
{
    auto game = game_websockets.find(guid);
    auto sender = getSocket(guid, puid);
    if (game != game_websockets.end() && sender != nullptr)
    {
        auto peer = game->second.find(puid);
        if (peer != game->second.end())
        {
            game->second.erase(peer);
            if (game->second.size() == 0)
                game_websockets.erase(game);
            else
            {
                {
                    auto ps_joined = UserJoined::createShared();
                    ps_joined->name = sender->m_data.username;
                    ps_joined->wins = sender->m_data.wins;
                    ps_joined->role = sender->m_data.role;

                    if (sender->m_data.role == e_Roles::HOST)
                    {
                        auto new_host = game->second.begin()->second;

                        for (const auto&[k, v] : game->second)
                            if (v != sender && v != new_host)
                                v->websocket.sendOneFrameText(createMessage(e_ZRPOpCodes::NEW_HOST, "{\"username\": \"" + new_host->m_data.username + "\"}"));
                        new_host->websocket.sendOneFrameText(createMessage(e_ZRPOpCodes::YOU_ARE_HOST_NOW, "{}"));
                    }

                    auto out = createMessage((sender->m_data.role == (int)e_Roles::SPECTATOR) ? (int)e_ZRPOpCodes::SPECTATOR_LEFT : (int)e_ZRPOpCodes::PLAYER_LEFT, json_mapper->writeToString(ps_joined));

                    for (const auto&[k, v] : game->second)
                        if (v != sender)
                            v->websocket.sendOneFrameText(out);
                }
            }
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

        auto out = createMessage((int)e_ZRPOpCodes::RECEIVE_MESSAGE, json_mapper->writeToString(message));
        for (const auto&[k, v] : game->second)
            v->websocket.sendOneFrameText(out);
    }
    else
        logger->log->error("No Player with ID: {} or game with ID: {} found!", puid, guid);
}

void ZRPConnector::getAllPlayersInLobby(uint32_t guid, uint32_t puid)
{
    auto sender = getSocket(guid, puid);
    auto game = game_websockets.find(guid);

    if (sender != nullptr && game != game_websockets.end())
    {
        auto players = PlayersInLobby::createShared();
        players->players = {};
        for (const auto&[k, v] : game->second)
        {
            if (v != nullptr)
            {
                auto p = ZwooUser::createShared();
                p->name = v->m_data.username;
                p->wins = v->m_data.wins;
                p->role = v->m_data.role;
                players->players->push_back(p);
            }
        }

        auto out = createMessage((int)e_ZRPOpCodes::ALL_PLAYERS_IN_LOBBY, json_mapper->writeToString(players));
        sender->websocket.sendOneFrameText(out);
    }
}

void ZRPConnector::leaveGame(uint32_t guid, uint32_t puid)
{
    auto game = game_websockets.find(guid);
    auto sender = getSocket(guid, puid);
    if (game != game_websockets.end() && sender != nullptr)
    {
        sender->websocket.sendClose(1000, "");
    }
    else
        logger->log->critical("No peer with ID {0} found can not remove WebSocket!", puid);
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
