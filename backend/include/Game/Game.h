#ifndef _GAME_H
#define _GAME_H_

#include <vector>
#include "Player.h"

namespace Backend::Game
{
    struct GameSettings;

    class Game
    {
    public:
        Game(/* args */);
        ~Game() {}

    public:
        const char *game_name = "";
        std::vector<Player> v_players;
        int game_id;

        GameSettings *s_settings;
    };

    struct GameSettings
    {
        int max_player = 2;
    };

} // namespace Backend::Game

#endif