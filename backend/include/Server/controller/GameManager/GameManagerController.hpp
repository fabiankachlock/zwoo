#ifndef _GAMEMANAGER_CONTROLLER_HPP_
#define _GAMEMANAGER_CONTROLLER_HPP_

#include "Server/controller/Authentication/AuthenticationController.hpp"
#include "Server/controller/error.h"
#include "Server/dto/GameManagerDTO.hpp"
#include "oatpp-websocket/Handshaker.hpp"
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include <boost/beast/core/detail/base64.hpp>
#include <string>
#include <functional>
#include <unordered_map>
#include <vector>

#include OATPP_CODEGEN_BEGIN( ApiController ) // <- Begin Codegen

struct s_Game
{
    std::string name;
    //                   puid  ,  role
    std::unordered_map<uint32_t, uint8_t>
        player; // Only Players who can join not all players

    std::string password;
    bool is_private;
};

class GameManagerController : public oatpp::web::server::api::ApiController
{
  private:
    OATPP_COMPONENT( std::shared_ptr<Logger>, m_logger_backend, "Backend" );
    OATPP_COMPONENT( std::shared_ptr<Logger>, m_logger_websocket, "Websocket" );
    OATPP_COMPONENT( std::shared_ptr<Database>, m_database );
    OATPP_COMPONENT( std::shared_ptr<GameManager>, game_manager );
    OATPP_COMPONENT( std::shared_ptr<ZwooAuthorizationHandler>, authHandler );
    OATPP_COMPONENT( std::shared_ptr<oatpp::network::ConnectionHandler>,
                     websocketConnectionHandler, "websocket" );

    std::unordered_map<uint32_t, s_Game> games;

    void printGames( )
    {
        for ( const auto &[ k2, v2 ] : games )
        {
            m_logger_backend->log->info( "{0}:", k2 );
            for ( const auto &[ k1, v1 ] : v2.player )
                m_logger_backend->log->info( "  {0}: {1},", k1, v1 );
        }
    }

    std::function<void(uint32_t guid)> remove_game = [&] (uint32_t guid) {
        games.erase(games.find(guid));
    };

  public:
    GameManagerController( const std::shared_ptr<ObjectMapper> &objectMapper )
        : oatpp::web::server::api::ApiController( objectMapper )
    {
        setDefaultAuthorizationHandler( authHandler );
        game_manager->after_game_removed = remove_game;
    }

    static std::shared_ptr<GameManagerController> createShared(
        OATPP_COMPONENT( std::shared_ptr<ObjectMapper>,
                         objectMapper ) // Inject objectMapper component here as
                                        // default parameter
    )
    {
        return std::make_shared<GameManagerController>( objectMapper );
    }

    ENDPOINT( "POST", "game/join", join_game,
              AUTHORIZATION( std::shared_ptr<UserAuthorizationObject>, usr ),
              BODY_DTO( Object<JoinGameDTO>, data ) )
    {
        m_logger_backend->log->info( "/POST join" );
        if ( data->opcode == 0 )
            return createResponse(
                Status::CODE_401,
                constructErrorMessage( "Opcode Missing",
                                       e_Errors::OPCODE_MISSING ) );
        else if ( data->opcode == 1 )
        {
            if ( data->name.getValue( "" ) == "" )
                return createResponse(
                    Status::CODE_401,
                    constructErrorMessage( "Game Name Missing",
                                           e_Errors::GAME_NAME_MISSING ) );
        }
        else if ( data->opcode == 2 || data->opcode == 3 )
        {
            if ( data->guid == 0 )
                return createResponse(
                    Status::CODE_401,
                    constructErrorMessage( "Gameid 0 is not valid!",
                                           e_Errors::INVALID_GAMEID ) );
        }
        else
            return createResponse(
                Status::CODE_401,
                constructErrorMessage( "Invalid Opcode",
                                       e_Errors::INVALID_OPCODE ) );

        if ( usr )
        {
            uint32_t guid = 0;

            if ( data->opcode == 1 )
            {
                guid = game_manager->createGame( );
                m_logger_backend->log->info( "New Game Created!" );
                s_Game g = { data->name.getValue( "" ),
                             { { usr->puid, 1 } },
                             data->password.getValue( "" ),
                             data->use_password };
                games.insert( { guid, g } );
                printGames( );
            }
            else
            {
                guid = data->guid;
                auto game = games.find( guid );
                if ( game != games.end( ) )
                {
                    if ( game->second.is_private )
                        if ( game->second.password !=
                             data->password.getValue( "" ) )
                            return createResponse(
                                Status::CODE_401,
                                constructErrorMessage(
                                    "Password not matching!",
                                    e_Errors::PASSWORD_NOT_MATCHING ) );

                    auto p = game->second.player.find( usr->puid );
                    if ( p == game->second.player.end( ) )
                        game->second.player.insert(
                            { usr->puid, data->opcode } );
                    else
                        return createResponse(
                            Status::CODE_401,
                            constructErrorMessage( "Already in this Game!",
                                                   e_Errors::ALREADY_INGAME ) );
                    printGames( );
                }
            }
            return createResponse( Status::CODE_200,
                                   "{\"guid\":\"" + std::to_string( guid ) +
                                       "\"}" );
        }
        return createResponse( Status::CODE_501, R"({"code": 100})" );
    }
    ENDPOINT_INFO( join_game )
    {
        info->summary = "An Endpoint to join a game.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );

        info->addSecurityRequirement( "Cookie" );
    }

    ENDPOINT( "GET", "game/join/{guid}", join,
              AUTHORIZATION( std::shared_ptr<UserAuthorizationObject>, usr ),
              REQUEST( std::shared_ptr<IncomingRequest>, request ),
              PATH( UInt32, guid, "guid" ) )
    {
        m_logger_backend->log->info( "/GET join" );
        printGames( );

        uint8_t role = 0;
        auto game = games.find( guid );
        if ( game != games.end( ) )
        {
            auto p = game->second.player.find( usr->puid );
            if ( p != game->second.player.end( ) )
            {
                role = p->second;
                game->second.player.erase( p );
            }
            else
                return createResponse(
                    Status::CODE_401,
                    constructErrorMessage( "Can't join game!",
                                           e_Errors::JOIN_FAILED ) );
        }
        else
            return createResponse(
                Status::CODE_404,
                constructErrorMessage( "Game Not Found!",
                                       e_Errors::GAME_NOT_FOUND ) );

        if ( usr )
        {
            m_logger_backend->log->info( "Player joined game" );
            auto res = oatpp::websocket::Handshaker::serversideHandshake(
                request->getHeaders( ), websocketConnectionHandler );
            auto parameters = std::make_shared<
                oatpp::network::ConnectionHandler::ParameterMap>( );
            ( *parameters )[ "puid" ] = std::to_string( usr->puid );
            ( *parameters )[ "username" ] = usr->username;
            ( *parameters )[ "guid" ] = std::to_string( guid );
            ( *parameters )[ "role" ] = std::to_string( role );
            ( *parameters )[ "wins" ] = std::to_string( (int32_t)usr->wins );
            res->setConnectionUpgradeParameters( parameters );
            return res;
        }
        return createResponse( Status::CODE_501, R"({"code": 100})" );
    }
    ENDPOINT_INFO( join )
    {
        info->summary = "An Endpoint to establish a Websocket connection.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );

        info->addSecurityRequirement( "Cookie" );
    }

    ENDPOINT( "GET", "game/leaderboard", leaderboard )
    {
        m_logger_backend->log->info( "/GET leaderboard" );
        return createDtoResponse( Status::CODE_200,
                                  m_database->getLeaderBoard( ) );
    }
    ENDPOINT_INFO( leaderboard )
    {
        info->summary = "An Endpoint to get the top 100.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }

    ENDPOINT( "GET", "game/games/", get_games )
    {
        m_logger_backend->log->info( "/GET Games" );

        auto ret = GetGameDTO::createShared();
        for (auto& [k, v]: games)
        {
            auto game = oatpp::Object<GameDTO>::createShared();
            game->name = v.name;
            game->id = k;
            game->isPublic = !v.is_private;
            game->playerCount = game_manager->getGame(k)->getPlayerCount();
            ret->games->push_back(game);
        }
        return createDtoResponse(Status::CODE_200, ret);
    }
    ENDPOINT_INFO( get_games )
    {
        info->summary = "An Endpoint to get all games.";

        info->addResponse<Object<GetGameDTO>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }
};

#include OATPP_CODEGEN_END( ApiController ) // <- End Codegen

#endif
