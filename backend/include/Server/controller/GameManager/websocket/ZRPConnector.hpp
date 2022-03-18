#ifndef _ZRPCONNECTOR_HPP_
#define _ZRPCONNECTOR_HPP_

#include "oatpp-websocket/ConnectionHandler.hpp"

#include "Server/dto/ZRPMessageDTO.hpp"

#include <unordered_map>
#include <functional>
#include <string>

enum e_ZRPOpCodes {
    PLAYER_JOINED = 100,
    SPECTATOR_JOINED = 101,
    PLAYER_LEFT = 102,
    SPECTATOR_LEFT = 103,
    SEND_MESSAGE = 104,
    RECEIVE_MESSAGE = 105
};

class ZRPConnector {
public:
    ZRPConnector();

    void sendMessage(uint32_t guid, uint32_t puid, std::string data);
};

#endif // _ZRPCONNECTOR_HPP_
