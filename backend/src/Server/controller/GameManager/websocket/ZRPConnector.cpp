#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"

#include <algorithm>

#include "Server/controller/GameManager/websocket/ZRPCodes.h"
#include "Server/dto/ZRPMessageDTO.hpp"

ZRPConnector::ZRPConnector( )
{
    logger = std::make_shared<Logger>( );
    logger->init( "ZRP" );

    json_mapper = oatpp::parser::json::mapping::ObjectMapper::createShared( );
}

void ZRPConnector::addWebSocket( uint32_t guid, uint32_t puid,
                                 std::shared_ptr<ZwooListener> listener )
{
    auto game = game_websockets.find( guid );
    if ( game != game_websockets.end( ) )
    {
        if ( game->second[ puid ] != nullptr )
            game->second[ puid ]->websocket.sendClose( 1005, "" );
        game->second[ puid ] = listener;
    }
    else
        game_websockets[ guid ] = { { puid, listener } };

    printWebsockets( );

    game = game_websockets.find( guid );

    {
        auto ps_joined = UserJoined::createShared( );
        ps_joined->name = listener->m_data.username;
        ps_joined->wins = listener->m_data.wins;
        ps_joined->role = listener->m_data.role;

        auto out =
            createMessage( ( listener->m_data.role == (int)e_Roles::SPECTATOR )
                               ? (int)e_ZRPOpCodes::SPECTATOR_JOINED
                               : (int)e_ZRPOpCodes::PLAYER_JOINED,
                           json_mapper->writeToString( ps_joined ) );
        sendZRPMessageToGame( guid, puid, out );
    }
}

void ZRPConnector::removeWebSocket( uint32_t guid, uint32_t puid )
{
    auto game = game_websockets.find( guid );
    auto sender = getSocket( guid, puid );
    if ( game != game_websockets.end( ) && sender != nullptr )
    {
        auto peer = game->second.find( puid );
        if ( peer != game->second.end( ) )
        {
            game->second.erase( peer );
            if ( game->second.size( ) == 0 )
                game_websockets.erase( game );
            else
            {
                {
                    auto ps_joined = UserJoined::createShared( );
                    ps_joined->name = sender->m_data.username;
                    ps_joined->wins = sender->m_data.wins;
                    ps_joined->role = sender->m_data.role;

                    if ( sender->m_data.role == e_Roles::HOST )
                    {
                        std::shared_ptr<ZwooListener> new_host;
                        for ( const auto &[ k, v ] : game->second )
                            if ( v != sender &&
                                 v->m_data.role == e_Roles::PLAYER )
                            {
                                new_host = v;
                                break;
                            }

                        sendZRPMessageToGame(
                            guid, new_host->m_data.puid,
                            createMessage( e_ZRPOpCodes::NEW_HOST,
                                           "{\"username\": \"" +
                                               new_host->m_data.username +
                                               "\"}" ) );

                        new_host->websocket.sendOneFrameText( createMessage(
                            e_ZRPOpCodes::YOU_ARE_HOST_NOW, "{}" ) );
                    }

                    auto out = createMessage(
                        ( sender->m_data.role == (int)e_Roles::SPECTATOR )
                            ? (int)e_ZRPOpCodes::SPECTATOR_LEFT
                            : (int)e_ZRPOpCodes::PLAYER_LEFT,
                        json_mapper->writeToString( ps_joined ) );

                    sendZRPMessageToGame( guid, 0, out );
                }
            }
            printWebsockets( );
        }
        else
            logger->log->critical(
                "No peer with ID {0} found can not remove WebSocket!", puid );
    }
    else
        logger->log->critical(
            "No game with GameID {0} found can not remove WebSocket!", guid );
}

void ZRPConnector::sendMessage( uint32_t guid, uint32_t puid, std::string data )
{
    auto send_message = json_mapper->readFromString<oatpp::Object<SendMessage>>(
        removeZRPCode( data ) );
    auto sender = getSocket( guid, puid );
    auto game = game_websockets.find( guid );

    if ( sender != nullptr && game != game_websockets.end( ) )
    {
        auto message = ReceiveMessage::createShared( );
        message->message = send_message->message;
        message->name = sender->m_data.username;
        message->role = sender->m_data.role;

        auto out = createMessage( (int)e_ZRPOpCodes::RECEIVE_MESSAGE,
                                  json_mapper->writeToString( message ) );
        sendZRPMessageToGame( guid, 0, out );
    }
    else
        logger->log->error( "No Player with ID: {} or game with ID: {} found!",
                            puid, guid );
}

void ZRPConnector::getAllPlayersInLobby( uint32_t guid, uint32_t puid )
{
    auto sender = getSocket( guid, puid );
    auto game = game_websockets.find( guid );

    if ( sender != nullptr && game != game_websockets.end( ) )
    {
        auto players = PlayersInLobby::createShared( );
        players->players = { };
        for ( const auto &[ k, v ] : game->second )
        {
            if ( v != nullptr )
            {
                auto p = ZwooUser::createShared( );
                p->name = v->m_data.username;
                p->wins = v->m_data.wins;
                p->role = v->m_data.role;
                players->players->push_back( p );
            }
        }

        auto out = createMessage( (int)e_ZRPOpCodes::ALL_PLAYERS_IN_LOBBY,
                                  json_mapper->writeToString( players ) );
        sender->websocket.sendOneFrameText( out );
    }
}

void ZRPConnector::leaveGame( uint32_t guid, uint32_t puid )
{
    auto game = game_websockets.find( guid );
    auto sender = getSocket( guid, puid );
    if ( game != game_websockets.end( ) && sender != nullptr )
    {
        sender->websocket.sendClose( 1000, "" );
    }
    else
        logger->log->critical(
            "No peer with ID {0} found can not remove WebSocket!", puid );
}

void ZRPConnector::kickPlayer( uint32_t guid, uint32_t puid, std::string data )
{
    auto player = json_mapper->readFromString<oatpp::Object<KickPlayer>>(
        removeZRPCode( data ) );
    auto sender = getSocket( guid, puid );

    if ( sender->m_data.role != e_Roles::HOST )
        return; // TODO: Send Error
    if ( sender == nullptr )
        return; // TODO: Send Error

    auto player_socket = getSocket( guid, player->username );
    if ( player_socket == nullptr )
        return; // TODO: Send Error
    leaveGame( guid, player_socket->m_data.puid );
}

void ZRPConnector::spectatorToPlayer( uint32_t guid, uint32_t puid )
{
    auto sender = getSocket( guid, puid );
    if ( sender != nullptr )
    {
        if ( sender->m_data.role == e_Roles::SPECTATOR && false /* In-Game */ )
        {
            sender->m_data.role_next_round = e_Roles::PLAYER;
        }
        else if ( sender->m_data.role == e_Roles::SPECTATOR &&
                  true /* not In-Game */ )
        {
            sendZRPMessageToGame(
                guid, puid,
                createMessage( e_ZRPOpCodes::PLAYER_CHANGED_ROLE,
                               "{\"username\": \"" + sender->m_data.username +
                                   "\", \"role\": " +
                                   std::to_string( (int)e_Roles::PLAYER ) +
                                   "}" ) );
        }
    }
}

void ZRPConnector::playerToSpectator( uint32_t guid, uint32_t puid )
{
    auto sender = getSocket( guid, puid );
    if ( sender != nullptr )
    {
        if ( ( sender->m_data.role == e_Roles::PLAYER ||
               sender->m_data.role == e_Roles::HOST ) ||
             true /* not in Game */ )
        {
            if ( sender->m_data.role == e_Roles::HOST )
            {
                auto game = getGame( guid );
                std::shared_ptr<ZwooListener> new_host;
                for ( const auto &[ k, v ] : game )
                    if ( v != sender && v->m_data.role == e_Roles::PLAYER )
                    {
                        new_host = v;
                        break;
                    }
                playerToHost( puid, guid, new_host->m_data.username );
            }
            sender->m_data.role = e_Roles::SPECTATOR;

            sendZRPMessageToGame(
                guid, puid,
                createMessage( e_ZRPOpCodes::PLAYER_CHANGED_ROLE,
                               "{\"username\": \"" + sender->m_data.username +
                                   "\", \"role\": " +
                                   std::to_string( (int)e_Roles::SPECTATOR ) +
                                   "}" ) );
        }
        else if ( false /* In-Game */ )
        {
            sender->m_data.role_next_round = e_Roles::SPECTATOR;
        }
    }
}

void ZRPConnector::playerToHost( uint32_t guid, uint32_t puid,
                                 std::string data )
{
    auto player = json_mapper->readFromString<oatpp::Object<PlayerToHost>>(
        removeZRPCode( data ) );
    auto sender = getSocket( guid, puid );

    if ( sender->m_data.role != e_Roles::HOST )
        return; // TODO: Send Error
    if ( sender == nullptr )
        return; // TODO: Send Error

    auto player_socket = getSocket( guid, player->username );
    if ( player_socket == nullptr )
        return; // TODO: Send Error

    if ( player_socket->m_data.role == e_Roles::PLAYER )
    {
        player_socket->m_data.role = e_Roles::HOST;
        sender->m_data.role = e_Roles::PLAYER;
        auto game = game_websockets.find( guid );
        if ( game != game_websockets.end( ) )
        {
            auto msg = createMessage(
                e_ZRPOpCodes::NEW_HOST,
                "{\"username\": \"" + player_socket->m_data.username + "\"}" );
            sendZRPMessageToGame( guid, player_socket->m_data.puid, msg );

            player_socket->websocket.sendOneFrameText(
                createMessage( e_ZRPOpCodes::YOU_ARE_HOST_NOW, "{}" ) );
        }
    }
}

void ZRPConnector::printWebsockets( )
{
    for ( const auto &[ k1, v1 ] : game_websockets )
    {
        logger->log->info( "{0}:", k1 );
        for ( const auto &[ k2, v2 ] : v1 )
            logger->log->info( "  {0}: {1},", k2,
                               v2->m_data.username.getValue( "" ) );
    }
}

void ZRPConnector::sendZRPMessageToGame( uint32_t guid, uint32_t puid_exclude,
                                         std::string message )
{
    auto exclude = getSocket(
        guid, puid_exclude ); // nullptr when message to all -> puid = 0
    auto game = getGame( guid );
    if ( !game.empty( ) )
        std::for_each( game.begin( ), game.end( ),
                       [ message, exclude ]( auto i )
                       {
                           if ( i.second != exclude )
                               i.second->websocket.sendOneFrameText( message );
                       } );
}

std::string ZRPConnector::removeZRPCode( std::string data )
{
    return data.substr( 4, data.size( ) - 4 );
}

std::string ZRPConnector::createMessage( int code, std::string data )
{
    return std::to_string( code ) + "," + data;
}

std::shared_ptr<ZwooListener> ZRPConnector::getSocket( uint32_t guid,
                                                       std::string name )
{
    auto game = game_websockets.find( guid );
    if ( game != game_websockets.end( ) )
    {
        for ( const auto &[ k1, v1 ] : game->second )
            if ( v1->m_data.username == name )
                return v1;
    }
    return nullptr;
}

std::shared_ptr<ZwooListener> ZRPConnector::getSocket( uint32_t guid,
                                                       uint32_t puid )
{
    auto game = game_websockets.find( guid );
    if ( game != game_websockets.end( ) )
    {
        auto socket = game->second.find( puid );
        if ( socket != game->second.end( ) )
            return socket->second;
    }
    return nullptr;
}

std::unordered_map<uint32_t, std::shared_ptr<ZwooListener>>
ZRPConnector::getGame( uint32_t guid )
{
    auto game = game_websockets.find( guid );
    if ( game != game_websockets.end( ) )
        return game->second;
    return std::unordered_map<uint32_t, std::shared_ptr<ZwooListener>>( );
}
