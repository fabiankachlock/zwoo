#include <unordered_map>
#include <vector>
#include <string>

#include "GameLogic/gamemanager.h"

// GAMEMANAGER //
GameManager::GameManager() {}
GameManager::~GameManager() {}


uint32_t GameManager::createGame() {

	std::shared_ptr<Game> gameptr = std::make_shared<Game>(Game()); // create game
	uint32_t GID = gameptr->getID();
	games.insert({ GID, gameptr }); // add game to games(unordered map)
	return GID;
}

void GameManager::destroyGame(uint32_t _GID) {
	// remove game from unordered map(games)
	games.erase(_GID);
}

std::shared_ptr<Game> GameManager::getGame(uint32_t _GID) {
	return this->games.at(_GID);
}

std::shared_ptr<Player> GameManager::getPlayer(uint32_t _GID, uint32_t _PUID) {
	return this->games.at(_GID)->getPlayer(_PUID);
}


