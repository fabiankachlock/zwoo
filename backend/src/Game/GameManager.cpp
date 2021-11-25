#include "Game/GameManager.h"

namespace Backend::Game
{
    GameManager::GameManager()
    = default;

    int GameManager::AddGame(Game* game)
    {
        game->game_id = GetNewUID();
        games.push_back(game);
        return game->game_id;
    }

    Game* GameManager::GetGame(const int* game_id)
    {
        for (auto game: games)
            if (game->game_id == *game_id)
                return game;
        return nullptr;
    }

    // TODO: Rework not very clean
    bool GameManager::DeleteGame(const int* game_id)
    {
        int i = 0;
        Game* game;
        for (auto it = games.begin(); it != games.end(); ++it)
        {
            game = games.at(i);
            if (game->game_id == *game_id)
            {
                games.erase(it);
                game->~Game();
                return true;
            }
            i++;
        }
        return false;
    }

    // TODO: Better System for generating UID
    int GameManager::GetNewUID()
    {
        ++tmp;
        return tmp;
    }

    
} // namespace Backend::Game
