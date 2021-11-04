#ifndef _GAMEMANAGER_H_
#define _GAMEMANAGER_H_

#include <vector>

namespace Backend::Game
{

    class GameManager
    {
    public:
        GameManager(/* args */);
        ~GameManager();
    private:
        void AddGame();
    };

} // namespace Backend::Game

#endif