#include "Game/GameManager.h"

namespace Backend::Game
{
    GameManager::GameManager()
    {
    }

    int GameManager::AddGame(Game* game)
    {
        game->game_id = GetNewUID();
        games.push_back(game);
        return game->game_id;
    }

    Game* GameManager::GetGame(int* game_id)
    {
        Game* game;
        for (int i = 0; i <= games.size(); i++)
        {
            game = games.at(i);
            if (game->game_id == *game_id)
                return game; 
        }
        return NULL;
    }

    int GameManager::GetNewUID()
    {
        ++tmp;
        return tmp;
    }

    
} // namespace Backend::Game
