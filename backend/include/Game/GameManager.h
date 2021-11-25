#ifndef _GAMEMANAGER_H_
#define _GAMEMANAGER_H_

#include <vector>
#include <algorithm>

#include "Game/Game.h"

namespace Backend::Game
{

    class GameManager
    {
    public:
        GameManager();
        ~GameManager() {}

        int AddGame(Game* game);
        Game* GetGame(const int* game_id);
        bool DeleteGame(const int* game_id);

        std::vector<Game*> games;
    private:
        int GetNewUID();

        int tmp = 0;
    };

} // namespace Backend::Game

#endif