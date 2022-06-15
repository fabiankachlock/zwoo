#include <string>
#include <unordered_map>
#include <vector>

#include "GameLogic/gamemanager.h"

// GAMEMANAGER //
GameManager::GameManager( std::shared_ptr<Logger> logger ) : logger( logger ) {}
GameManager::~GameManager( ) {}

uint32_t GameManager::createGame( )
{
    auto GID = id_generator.GetID( );
    auto res = games.find( GID );
    while ( res != nullptr ) // just for safety
    {
        GID = id_generator.GetID( );
        res = games.find( GID );
    }

    std::shared_ptr<Game> gameptr =
        std::make_shared<Game>( GID ); // create game
    games.insert( { GID, gameptr } );  // add game to games(unordered map)
    logger->log->info( "Created Game! ID: {}", GID );
    return GID;
}

void GameManager::destroyGame( uint32_t _GID )
{
    // remove game from unordered map(games)
    games.erase( _GID );
    logger->log->info( "Game {} deleted", _GID );
}

std::shared_ptr<Game> GameManager::getGame( uint32_t _GID )
{
    return this->games.at( _GID );
}

std::shared_ptr<Player> GameManager::getPlayer( uint32_t _GID, uint32_t _PUID )
{
    return this->games.at( _GID )->getPlayer( _PUID );
}
