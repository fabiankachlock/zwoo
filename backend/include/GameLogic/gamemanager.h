#pragma once

#include <map>
#include <string>
#include <unordered_map>
#include <vector>

#include "Helper.h"
#include "Server/logger/logger.h"

#include "card.h"
#include "game.h"
#include "player.h"
#include "rule.h"

class Game;
class GameManager;

// maintains all games
// acts as interface between incoming ZRP-codes and GameLogic
class GameManager
{
  private:
    std::unordered_map<uint32_t, std::shared_ptr<Game>> games;

    std::shared_ptr<Logger> logger;
    UIDGenerator id_generator = UIDGenerator( 0 );

  public:
    GameManager( std::shared_ptr<Logger> logger );
    ~GameManager( );

    // create empty game
    uint32_t createGame( );

    void destroyGame( uint32_t _GID );

    // adds player to active game
    std::shared_ptr<Game> getGame( uint32_t _GID );

    std::shared_ptr<Player> getPlayer( uint32_t _GID, uint32_t _PUID );
};