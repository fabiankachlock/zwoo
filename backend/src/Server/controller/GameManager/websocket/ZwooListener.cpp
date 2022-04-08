#include "Server/controller/GameManager/websocket/ZwooListener.hpp"

#include "Server/controller/GameManager/websocket/ZRPCodes.h"
#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"

#include <iostream>

void ZwooListener::onPing( const WebSocket &socket,
                           const oatpp::String &message )
{
    socket.sendPong( message );
}

void ZwooListener::onPong( const WebSocket &socket,
                           const oatpp::String &message )
{
}

void ZwooListener::onClose( const WebSocket &socket, v_uint16 code,
                            const oatpp::String &message )
{
    logger->log->debug( "socket closed! Code: {0}", code );
}

void ZwooListener::readMessage( const WebSocket &socket, v_uint8 opcode,
                                p_char8 data, oatpp::v_io_size size )
{
    if ( size == 0 )
    { // message transfer finished
        auto wholeMessage = m_messageBuffer.toString( );
        m_messageBuffer.setCurrentPosition( 0 );

        std::string scode = wholeMessage->substr( 0, 3 );
        int code = ( ( (int)scode[ 0 ] - 48 ) * 100 ) +
                   ( ( (int)scode[ 1 ] - 48 ) * 10 ) + ( (int)scode[ 2 ] - 48 );

        switch ( code )
        {
        case e_ZRPOpCodes::SEND_MESSAGE:
            connector->sendMessage( m_data.guid, m_data.puid, wholeMessage );
            break;
        case e_ZRPOpCodes::GET_ALL_PLAYERS_IN_LOBBY:
            connector->getAllPlayersInLobby( m_data.guid, m_data.puid );
            break;
        case e_ZRPOpCodes::PLAYER_LEAVE:
            connector->leaveGame( m_data.guid, m_data.puid );
            break;
        case e_ZRPOpCodes::SPECTATOR_TO_PLAYER:
            connector->spectatorToPlayer( m_data.guid, m_data.puid );
            break;
        case e_ZRPOpCodes::PLAYER_TO_SPECTATOR:
            connector->playerToSpectator( m_data.guid, m_data.puid );
            break;
        case e_ZRPOpCodes::PLAYER_TO_HOST: // Maybe username in body
            break;
        case e_ZRPOpCodes::KICK_PLAYER:
            connector->kickPlayer( m_data.guid, m_data.puid, wholeMessage );
            break;
        default:
            logger->log->debug( "Unknown Code {0}!", code );
            break;
        }
    }
    else if ( size > 0 )
    { // message frame received
        m_messageBuffer.writeSimple( data, size );
    }
}
