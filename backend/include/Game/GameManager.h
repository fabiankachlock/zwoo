#ifndef _GAMEMANAGER_H_
#define _GAMEMANAGER_H_

#include <vector>

#include "Game/Game.h"

namespace Backend::Game
{

    class GameManager
    {
    public:
        GameManager(/* args */);
        ~GameManager() {}

        int AddGame(Game* game);
        Game* GetGame(int* game_id);

        std::vector<Game*> games;
    private:
        int GetNewUID();

        int tmp = 0;
    };

} // namespace Backend::Game

#endif