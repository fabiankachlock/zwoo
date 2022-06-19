#include "Server/controller/GameManager/websocket/ZRPConnector.hpp"

#include <algorithm>
#include <sstream>

#include "Server/controller/GameManager/websocket/ZRPCodes.h"
#include "Server/dto/ZRPMessageDTO.hpp"

ZRPConnector::ZRPConnector( std::shared_ptr<GameManager> gm, std::shared_ptr<Database> db )
    : game_manager( gm ), database(db)
{
    logger = std::make_shared<Logger>( );
    logger->init( "ZRP" );

    json_mapper = oatpp::parser::json::mapping::ObjectMapper::createShared( );

    gm->next_turn = [&](uint32_t guid, uint32_t puid){
        auto socket = getSocket(guid, puid);
        if (socket != nullptr)
            socket->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::START_TURN, "{}"));
    };

    gm->end_turn = [&](uint32_t guid, uint32_t puid){
        auto socket = getSocket(guid, puid);
        if (socket != nullptr)
            socket->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::END_TURN, "{}"));
    };

    gm->get_card = [&](uint32_t guid, uint32_t puid, Card card) {
        auto c = CardDTO::createShared();
        c->type = card.color;
        c->symbol = card.number;
        auto socket = getSocket(guid, puid);
        if (socket != nullptr)
            socket->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::GET_CARD, json_mapper->writeToString(c)));
    };

    gm->remove_card = [&](uint32_t guid, uint32_t puid, Card card) {
        auto c = CardDTO::createShared();
        c->type = card.color;
        c->symbol = card.number;
        auto socket = getSocket(guid, puid);
        if (socket != nullptr)
            socket->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::REMOVE_CARD, json_mapper->writeToString(c)));
    };

    gm->state_changed = [&](uint32_t guid, Card card, Player activ, Player last) {
        auto dto = StateChangedDTO::createShared();
        auto c = oatpp::Object<CardDTO>::createShared();
        c->symbol = card.number;
        c->type = card.color;
        dto->pileTop = c;
        dto->activePlayer = getSocket(guid, activ.getID())->m_data.username;
        dto->activePlayerCardAmount = activ.getCardsOnHand();
        dto->lastPlayer = getSocket(guid, last.getID())->m_data.username;
        dto->lastPlayerCardAmount = last.getCardsOnHand();

        sendZRPMessageToGame(guid, 0, createMessage(e_ZRPOpCodes::STATE_UPDATE, json_mapper->writeToString(dto)));
    };

    gm->game_won = [&](uint32_t guid, uint32_t puid) {
        auto ret = PlayerWonDTO::createShared();
        ret->wins = database->incrementWins(puid);
        ret->username = getSocket(guid, puid)->m_data.username;

        game_manager->getGame(guid)->playerorder.for_each([&](auto i, std::shared_ptr<Player> p){
            auto ps = oatpp::Object<PlayerSummaryDTO>::createShared();
            uint32_t score = 0;
            for (auto card: p->cards)
                score += card->getValue();
            ps->score = score;
            ps->username = getSocket(guid, p->getID())->m_data.username;
            ret->summary->push_back(ps);
        });
        ret->summary->sort([&](auto a, auto b){ return b->score > a->score; });
        for (int i = 1; i < ret->summary->size(); i++)
            ret->summary[i]->position = i;
        sendZRPMessageToGame(guid, 0, createMessage(e_ZRPOpCodes::GAME_WON, json_mapper->writeToString(ret)));

    };
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
    listener->m_data.role_next_round = listener->m_data.role;
    if (game_manager->getGame(guid)->isActive())
    {
        listener->m_data.role = e_Roles::SPECTATOR;
    }
    if (listener->m_data.role != e_Roles::SPECTATOR)
        game_manager->getGame( guid )->addPlayer( puid );

    {
        auto ps_joined = UserJoined::createShared( );
        ps_joined->username = listener->m_data.username;
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
            {
                game_manager->destroyGame( guid );
                game_websockets.erase( game );
            }
            else
            {
                game_manager->getGame( guid )->removePlayer( puid );
                {
                    auto ps_joined = UserJoined::createShared( );
                    ps_joined->username = sender->m_data.username;
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
        message->username = sender->m_data.username;
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
                p->username = v->m_data.username;
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
    {
        sender->websocket.sendOneFrameText(
            createMessage( e_ZRPError::ACCESS_DENIED_ERROR,
                           R"({"message":"Access denied your not a host"})" ) );
        return;
    }
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
        if ( sender->m_data.role == e_Roles::SPECTATOR && game_manager->getGame(guid)->isActive() )
        {
            sender->m_data.role_next_round = e_Roles::PLAYER;
        }
        else if ( sender->m_data.role == e_Roles::SPECTATOR && !game_manager->getGame(guid)->isActive() )
        {
            sender->m_data.role = e_Roles::PLAYER;
            game_manager->getGame(guid)->addPlayer(puid);
            sendZRPMessageToGame(
                guid, 0,
                createMessage( e_ZRPOpCodes::PLAYER_CHANGED_ROLE,
                               R"({"username": ")" + sender->m_data.username +
                                   R"(", "role": )" +
                                   std::to_string( (int)e_Roles::PLAYER ) +
                                   "}" ) );
        }
    }
}

void ZRPConnector::playerToSpectator( uint32_t guid, uint32_t puid,
                                      std::string data )
{
    auto sender = getSocket( guid, puid );
    auto ptos = json_mapper->readFromString<oatpp::Object<PlayerToSpectator>>(
        removeZRPCode( data ) );

    if ( ptos->username.getValue( "" ) != "" )
    {
        if ( sender->m_data.role == e_Roles::HOST ||
             sender->m_data.username == ptos->username )
            sender = getSocket( guid, ptos->username.getValue( "" ) );
        else
            return; // or send error
    }

    if ( sender != nullptr )
    {
        if ( ( sender->m_data.role == e_Roles::PLAYER ||
               sender->m_data.role == e_Roles::HOST ) &&
               !game_manager->getGame(guid)->isActive())
        {
            game_manager->getGame(guid)->removePlayer(puid);
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
                playerToHost( puid, guid,
                              R"({"username": ")" + new_host->m_data.username +
                                  R"("})" );
            }
            sender->m_data.role = e_Roles::SPECTATOR;

            sendZRPMessageToGame(
                guid, 0,
                createMessage( e_ZRPOpCodes::PLAYER_CHANGED_ROLE,
                               "{\"username\": \"" + sender->m_data.username +
                                   "\", \"role\": " +
                                   std::to_string( (int)e_Roles::SPECTATOR ) +
                                   "}" ) );
        }
        else if ( game_manager->getGame(guid)->isActive() )
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
    {
        sender->websocket.sendOneFrameText(
            createMessage( e_ZRPError::ACCESS_DENIED_ERROR,
                           R"({"message":"Access denied your not a host"})" ) );
        return;
    }
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
                R"({"username": ")" + player_socket->m_data.username + "\"}" );
            sendZRPMessageToGame( guid, player_socket->m_data.puid, msg );

            player_socket->websocket.sendOneFrameText(
                createMessage( e_ZRPOpCodes::YOU_ARE_HOST_NOW, "{}" ) );
        }
    }
}

void ZRPConnector::changeSettings( uint32_t guid, uint32_t puid,
                                   std::string data )
{
    auto sender = getSocket( guid, puid );

    if ( sender->m_data.role != e_Roles::HOST )
    {
        sender->websocket.sendOneFrameText(
            createMessage( e_ZRPError::ACCESS_DENIED_ERROR,
                           R"({"message":"Access denied your not a host"})" ) );
        return;
    }
    if ( sender == nullptr )
        return; // TODO: Send Error

    sender->websocket.sendOneFrameText( createMessage(
        400, "changing settings is disabled for the first Beta" ) );
}

void ZRPConnector::getAllSettings( uint32_t guid, uint32_t puid )
{
    auto sender = getSocket( guid, puid );

    sender->websocket.sendOneFrameText( createMessage(
        203,
        R"({"settings": [{"setting": "maxPlayers", "value": 5},{"setting": "maxDraw","value": 108},{"setting": "maxCardsOnHand","value": 108},{"setting": "initialCards","value": 7}]})" ) );
}

void ZRPConnector::startGame( uint32_t guid, uint32_t puid )
{
    auto sender = getSocket( guid, puid );

    auto g = getGame(guid);
    for (auto [k,v]: g)
    {
        if (v->m_data.role != v->m_data.role_next_round)
        {
            if (v->m_data.role_next_round == e_Roles::PLAYER)
                spectatorToPlayer(v->m_data.guid, v->m_data.puid);
            else
                playerToSpectator(v->m_data.guid, v->m_data.puid, "111,{}");
        }
    }

    if ( sender == nullptr )
        return; // TODO: Send Error
    if ( sender->m_data.role != e_Roles::HOST )
    {
        sender->websocket.sendOneFrameText(
            createMessage( e_ZRPError::ACCESS_DENIED_ERROR,
                           R"({"message":"Access denied your not a host"})" ) );
        return;
    }
    if ( !game_manager->getGame( guid )->canStart( ) )
        return; // TODO: Send Error

    sendZRPMessageToGame( guid, 0, createMessage( e_ZRPOpCodes::GAME_STARTED, "{}" ) );
    game_manager->getGame( guid )->start( );
    getSocket(guid, game_manager->getGame(guid)->getCurPlayer()->getID())->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::START_TURN, "{}"));
}

void ZRPConnector::placeCard( uint32_t guid, uint32_t puid, std::string data )
{
    if (game_manager->getGame(guid)->getCurPlayer()->getID() != puid)
        return; // TODO: Send Error

    auto card = json_mapper->readFromString<oatpp::Object<CardDTO>>(
        removeZRPCode( std::move(data) ) );
    if (game_manager->getGame(guid)->placeCardEvent(Card(card->type, card->symbol)))
    {
        // Update Game state
        if (game_manager->getGame(guid)->containsExpectedAction(e_gaction::G_PLAYER_PICK))
            getSocket(guid, puid)->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::GET_PLAYER_DECISION, R"({"type": 1})"));
    }
    else
        return; // TODO: Send Error
}

void ZRPConnector::drawCard( uint32_t guid, uint32_t puid )
{
    if (game_manager->getGame(guid)->getCurPlayer()->getID() != puid)
        return; // TODO: Send Error

    game_manager->getGame(guid)->drawCardEvent();
}

void ZRPConnector::getHand( uint32_t guid, uint32_t puid )
{
    auto socket = getSocket(guid, puid);
    if (socket->m_data.role == e_Roles::SPECTATOR)
        return;
    auto player = game_manager->getGame(guid)->getPlayer(puid);

    if (player == nullptr)
        return; // TODO: Send Error
    if (socket == nullptr)
        return; // TODO: Send Error

    auto hand = PlayerHandDTO::createShared();

    for (auto c: player->cards)
    {
        auto card = oatpp::Object<CardDTO>::createShared();
        card->type = c->color;
        card->symbol = c->number;
        hand->hand->push_back(card);
    }

    socket->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::SEND_HAND, json_mapper->writeToString(hand)));
}

void ZRPConnector::getPlayerCardAmount( uint32_t guid, uint32_t puid )
{
    auto sender = getSocket(guid, puid);
    auto game = game_manager->getGame(guid);
    auto curr_player = game->getCurPlayer();
    auto ret = PlayerCardAmountDTO::createShared();
    game->playerorder.for_each([&](auto i, std::shared_ptr<Player> p) {
        auto player = oatpp::Object<PlayerDTO>::createShared();
        auto s = getSocket(guid, p->getID());
        player->username = s->m_data.username;
        player->cards = p->cards.size();
        player->isActivePlayer = p->getID() == curr_player->getID();
        ret->players->push_back(player);
    });
    sender->websocket.sendOneFrameText( createMessage(e_ZRPOpCodes::SEND_PLAYER_CARD_AMOUNT, json_mapper->writeToString(ret)));
}

void ZRPConnector::getStackTop( uint32_t guid, uint32_t puid )
{
    auto socket = getSocket( guid, puid );
    if ( socket != nullptr)
    {
        auto card = game_manager->getGame(guid)->getTopCard();
        auto c = CardDTO::createShared();
        c->type = card.color;
        c->symbol = card.number;
        socket->websocket.sendOneFrameText( createMessage( e_ZRPOpCodes::SEND_STACK_TOP, json_mapper->writeToString(c)) );
    }
}

void ZRPConnector::receivePlayerDecision( uint32_t guid, uint32_t puid, std::string data )
{
    auto decision = json_mapper->readFromString<oatpp::Object<PlayerDecisionDTO>>( removeZRPCode(data));

    switch ( decision->type )
    {
    case e_DecisionTypes::CHOOSE_COLOR:
        game_manager->getGame(guid)->wildEvent(decision->decision);
        break;
    default:
        logger->log->warn("Decision {} not found!", decision->type);
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
    return { };
}
