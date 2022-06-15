#pragma once
#include "card.h"
#include <memory>
#include <unordered_map>

struct CardRule
{
    uint8_t ID;      // RuleID
    Card cardtype;   // type of card that is affected
    e_action action; // Type of action performed by Cardtype
    uint8_t mod;     // modifier (depending on action e.g. draw +mod cards).

    CardRule( );
    CardRule( uint8_t _ID, e_action _action, Card _cardtype, uint8_t _mod );
};

struct GameRules
{
  private:
    uint8_t idcnt;
    std::unordered_map<std::string, uint8_t> gameoptions;

  public:
    GameRules( );

    /// <summary>
    /// maxplayers - maximum amount of players in one game
    /// maxDraw - how many cards can be drawn at max in a chain
    /// maxCardsOnHand - how many cards are allowed on one player's hand
    /// initialCards - How many cards does each player get to the beginning of
    /// the round
    /// </summary>

    std::unordered_map<uint8_t, std::shared_ptr<CardRule>>
        cardrules; // stores all cardrules (cards not listed in here are handled
                   // as Regular cards)

    /*
    type	-	typeid	-	typename	-	arg1	-	arg2	-
    arg3 -	arg4 typeid	-	1		-	gamerule
    - typeid
    -	2		-	cardrule	-	action	-	cardclr	-
    cardnbr -	mod
    */
    uint8_t createRule( int _type, uint16_t _arg1, uint16_t _arg2,
                        uint16_t _arg3,
                        uint16_t _arg4 ); // only used by initial gamecreation

    void activateRule( uint8_t id );
    void deactivateRule( uint8_t id );

    bool canPlace(
        Card _stacktopcard, Card _playerplacecard,
        bool _playerDraw ); // checks if card can be placed according to rules

    uint16_t canDraw(
        uint16_t _curDrawCount,
        uint8_t _cardsOnHand ); // returns the max amount the player can draw

    bool canDraw( uint8_t _cardsOnHand ); // checks for drawing one card

    bool checkSpecial( ); // checks for any special defined rules

    bool setOption( std::string _option, uint8_t _value );
    uint8_t getOption( std::string _option );

    CardRule getCardRule( Card _card ); // get Rules corresponding to card
                                        // according to activated rules

    bool checkMaxCardsOnHand( uint8_t _cardsdrawn, uint8_t _cards );
};
