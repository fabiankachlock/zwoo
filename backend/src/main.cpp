#include <iostream>

#include "Game/GameManager.h"

int main()
{
    auto gm = Backend::Game::GameManager();
    auto g1 = Backend::Game::Game();
    auto g2 = Backend::Game::Game();

    g1.game_name = "Test Game 1";
    g2.game_name = "Test Game 2";

    int game1 = gm.AddGame(&g1);
    int game2 = gm.AddGame(&g2);

    std::cout << gm.games.size() << std::endl;
    std::cout << "Game 1: " << gm.GetGame(&game1)->game_name << std::endl;
    std::cout << "Game 2: " << gm.GetGame(&game2)->game_name << std::endl;

    return 0;
}