#pragma once

#include <unordered_map>
#include <map>
#include <vector>
#include <string>

#include "card.h"
#include "player.h"
#include "rule.h"
#include "game.h"


class Game;
class GameManager;

// maintains all games
// acts as interface between incoming ZRP-codes and GameLogic
class GameManager {
private:
	std::unordered_map<uint32_t, std::shared_ptr<Game>> games;
public:
	GameManager();
	~GameManager();

	// create empty game
	uint32_t createGame();

	void destroyGame(uint32_t _GID);

	// adds player to active game
	std::shared_ptr<Game> getGame(uint32_t _GID);

	std::shared_ptr<Player> getPlayer(uint32_t _GID, uint32_t _PUID);

};
