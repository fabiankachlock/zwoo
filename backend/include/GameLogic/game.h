#pragma once

#include <map>
#include <string>
#include <unordered_map>
#include <vector>

#include "CyclingList.hpp"
#include "card.h"
#include "player.h"
#include "rule.h"

enum e_gaction
{
    G_NONE,
    G_START,           // START the game
    G_STOP,            // STOP the game
    G_OPTION_MOD,      // MODify a game option
    G_RULE_ADD,        // ADD a Rule to the game
    G_RULE_RMV,        // ReMoVe a Rule from the game
    G_RULE_MOD,        // MODify a Rule
    G_RULE_ACTIVATE,   // Activate a Rule
    G_RULE_DEACTIVATE, // Deactivate a Rule
    G_PLAYER_PLACE,    // place a card
    G_PLAYER_DRAW,     // draw card/cards
    G_PLAYER_PICK,     // pick a color
    G_PLAYER_ADD,      // ADD player to a game
    G_PLAYER_RMV       // ReMoVe player from a game
};

class Game
{
  private:
    GameRules rules;
    CardPile pile;
    CardStack stack;

    bool direction;     // 1 = clockwise 0 = counterclockwise
    bool active;        // is the game running or not
    bool draw_active;   // does player have to draw
    bool pick_active;   // does player have to pick
    uint32_t turncount; // current count of turns
    uint16_t
        current_draw_amount; // How much cards have to be drawn at the moment
    const uint32_t ID;       // GameID
    Card tmpCard;            // used for something like wild events

    CyclingList<std::shared_ptr<Player>>::iterator
        current_player; // currently active player
    CyclingList<std::shared_ptr<Player>> playerorder;

    std::unordered_map<uint32_t, std::shared_ptr<Player>>
        players; // all players anticipating in the game
    std::vector<e_gaction> expected_actions; // which order/orders does the game
                                             // expect to recieve next

    void drawCard( std::shared_ptr<Player> _player ); // draws a single card
    void drawCards( std::shared_ptr<Player> _player,
                    uint8_t _amount ); // draws multiple cards
    void placeCard( std::shared_ptr<Player> _player,
                    Card _card ); // places a card
    void placeCard( std::shared_ptr<Player> _player, Card _card1,
                    Card _card2 ); // places _card2 onto the stack but removes
                                   // _card1 from inventory

    void nextTurn( ); // issues next player
    bool reset( );

    bool isValidAction( e_gaction _action ); // is action expected?
    bool isWon( );                           // is the game won?

  public:
    Game( uint32_t id );
    ~Game( );

    bool start( );
    bool stop( );

    // GETTER //
    bool isActive( );
    uint8_t getPlayerCount( );
    uint32_t getID( );
    std::vector<uint32_t> getPlayerIDs( );
    std::shared_ptr<Player> getPlayer( uint32_t _PUID );
    std::shared_ptr<Player> getCurPlayer( );
    std::vector<e_gaction> getExpectedActions( );

    // EVENTS //
    void placeCardEvent( Card _card ); // player places a card
    void drawCardEvent( );             // lets player draw one or multiple cards
    void skipEvent( );                 // skip the next player
    void reverseEvent( );              // changes play direction of game
    void wildEvent( uint32_t _chosencolor ); // lets player choose a color

    bool setOption( std::string _option, uint8_t _value );
    uint8_t getOption( std::string _option );

    void PRINTSTATSPILE( );
    void PRINTSTATSSTACK( );

    uint32_t addPlayer( uint32_t puid );
    bool removePlayer( uint32_t _playerid );
};