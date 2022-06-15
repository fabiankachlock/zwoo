#ifndef _ZRPCONNECTOR_HPP_
#define _ZRPCONNECTOR_HPP_

#include "Server/controller/GameManager/websocket/ZwooListener.hpp"
#include "Server/dto/ZRPMessageDTO.hpp"
#include "oatpp-websocket/ConnectionHandler.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"

#include "GameLogic/gamemanager.h"

#include <functional>
#include <string>
#include <unordered_map>

class ZRPConnector
{
  public:
    ZRPConnector( std::shared_ptr<GameManager> gm );

    void addWebSocket( uint32_t guid, uint32_t puid,
                       std::shared_ptr<ZwooListener> listener );
    void removeWebSocket( uint32_t guid, uint32_t puid );

    void sendMessage( uint32_t guid, uint32_t puid, std::string data );

    // 1xx
    void getAllPlayersInLobby( uint32_t guid, uint32_t puid );

    void leaveGame( uint32_t guid, uint32_t puid );

    void kickPlayer( uint32_t guid, uint32_t puid, std::string data );
    void spectatorToPlayer( uint32_t guid, uint32_t puid );
    void playerToSpectator( uint32_t guid, uint32_t puid, std::string data );
    void playerToHost( uint32_t guid, uint32_t puid, std::string data );

    // 2xx
    void changeSettings( uint32_t guid, uint32_t puid, std::string data );
    void getAllSettings( uint32_t guid, uint32_t puid );
    void startGame( uint32_t guid, uint32_t puid );

  private:
    void printWebsockets( );
    void sendZRPMessageToGame( uint32_t guid, uint32_t puid_exclude,
                               std::string message );

    std::string removeZRPCode( std::string data );
    std::string createMessage( int code, std::string data );

    //  std::unordered_map<uint32_t, std::shared_ptr<ZwooListener>>
    //  getGame(guid);

    std::shared_ptr<GameManager> game_manager;

    std::shared_ptr<ZwooListener> getSocket( uint32_t guid, uint32_t puid );
    std::shared_ptr<ZwooListener> getSocket( uint32_t guid, std::string name );

    std::unordered_map<uint32_t, std::shared_ptr<ZwooListener>>
    getGame( uint32_t guid );

    //                   guid  ,                      puid  ,           player
    //                   Listener
    std::unordered_map<
        uint32_t, std::unordered_map<uint32_t, std::shared_ptr<ZwooListener>>>
        game_websockets;

    std::shared_ptr<Logger> logger;
    std::shared_ptr<oatpp::parser::json::mapping::ObjectMapper> json_mapper;
};

#endif // _ZRPCONNECTOR_HPP_
