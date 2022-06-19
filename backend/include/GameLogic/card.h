#pragma once
#include <algorithm>
#include <memory>
#include <random>
#include <vector>
// DEBUG
#include <iostream>
#include <string>

#include <spdlog/spdlog.h>

struct Card;
class CardPile;
class CardStack;

enum e_action
{
    A_NONE = 0,
    A_REGULAR = 1,
    A_SKIP = 2,
    A_DRAW = 3,
    A_REVERSE = 4,
    A_WILD = 5,
    A_WILD_FOUR = 6
};

enum e_color
{
    C_NONE = 0,
    C_RED = 1,
    C_YELLOW = 2,
    C_BLUE = 3,
    C_GREEN = 4,
    C_BLACK = 5
};

enum e_number
{
    N_NONE = 0,
    N_ZERO = 1,
    N_ONE = 2,
    N_TWO = 3,
    N_THREE = 4,
    N_FOUR = 5,
    N_FIVE = 6,
    N_SIX = 7,
    N_SEVEN = 8,
    N_EIGHT = 9,
    N_NINE = 10,
    N_SKIP = 11,
    N_REVERSE = 12,
    N_DRAW_TWO = 13,
    N_WILD = 14,
    N_WILD_FOUR = 15
};

struct Card
{
    e_color color;
    e_number number;
    Card( e_color _color, e_number _number ); // creates card
    Card( uint8_t _color, uint8_t _number );  // creates card

    uint32_t getValue( );

    Card( ); // creates NONE type card
    bool operator==( Card &_card );
};

class CardPile
{
  private:
    std::vector<std::shared_ptr<Card>> cards;
    uint16_t maxsize;

    // checks current size of pile and gets cards from stack if empty
    bool isEmpty( );

  public:
    CardPile( );
    ~CardPile( );

    void shuffle( );
    void generate( );
    bool reset( );

    // CREATES NEW card and adds it to the pile
    void createCard( Card _card );
    // DESTROYS card and remove it from pile
    void destroyCard( Card _card );

    // returns card from top and removes it from pile
    std::shared_ptr<Card> drawTopCard( );

    // adds EXISTING card to top of pile(end of vector)
    void addCard( std::shared_ptr<Card> _card );

    // adds multiple EXISTING cards to top of pile(end of vector)
    void addCards( std::vector<std::shared_ptr<Card>> _cards );

    // get all cards from CardStack (except last(top) card) and add them to the
    // pile
    void insertCardStack( std::shared_ptr<CardStack> _stack );

    // DEBUG
    void PRINTSTATS( );

    // TODO: REMOVE AFTER BETA
    std::shared_ptr<spdlog::logger> log;
};

class CardStack
{

  public:
    std::vector<std::shared_ptr<Card>> cards;

    CardStack( );

    // init CardStack with top card
    CardStack( std::shared_ptr<Card> _card );

    // adds card to top of stack
    void addCard( std::shared_ptr<Card> _card );

    Card getTopCard( );
    bool reset( );

    // get all cards except top one from stack
    std::vector<std::shared_ptr<Card>> drawBackCards( );

    // DEBUG
    void PRINTSTATS( );

    // TODO: REMOVE AFTER BETA
    std::shared_ptr<spdlog::logger> log;
};