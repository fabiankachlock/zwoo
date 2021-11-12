#ifndef _PLAYER_H_
#define _PLAYER_H_

#define CARD_RED    0
#define CARD_BLUE   1
#define CARD_YELLOW 2
#define CARD_GREEN  3

#define CARD_NULL  0
#define CARD_ONE   1
#define CARD_TWO   2
#define CARD_THREE 3
#define CARD_FOUR  4
#define CARD_FIVE  5
#define CARD_SIX   6
#define CARD_SEVEN 7
#define CARD_EIGHT 8
#define CARD_NINE  9
#define CARD_SKIP  10
#define CARD_FLIP  11
#define CARD_DRAW  12

#define CARD_SELECT 0
#define CARD_SELECT_DRAW2 1
#define CARD_SELECT_DRAW4 2

#include <string>
#include <vector>

typedef std::pair<int, int> Card;

namespace Backend::Game
{
    class Player
    {
    public:
        Player(char* name);
        ~Player() {}

    private:
        const char* name = "";
        std::vector<Card> cards;
    };

} // namespace Backend::Game

#endif