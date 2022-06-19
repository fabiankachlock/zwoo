#include "GameLogic/rule.h"
#include "GameLogic/card.h"
#include "GameLogic/player.h"

CardRule::CardRule( ) {}

CardRule::CardRule( uint8_t _ID, e_action _action, Card _cardtype,
                    uint8_t _mod )
    : ID( _ID ), action( _action ), cardtype( _cardtype ), mod( _mod )
{
}

GameRules::GameRules( )
{
    idcnt = 1;
    // init gameoptions
    gameoptions.insert( { "maxPlayers", 5 } );
    gameoptions.insert( { "maxDraw", 108 } );
    gameoptions.insert( { "maxCardsOnHand", 108 } );
    gameoptions.insert( { "initialCards", 9 } );
}

uint8_t GameRules::createRule( int _type, uint16_t _arg1, uint16_t _arg2,
                               uint16_t _arg3, uint16_t _arg4 )
{
    switch ( _type )
    {
    case 1: // GameRule
        break;
    case 2: // CardRule
        log->info( "CREATING A CARDRULE WITH FOLLOWING ARGUMENTS: {} {} {} {}",
                   _arg1, _arg2, _arg3, _arg4 );
        uint8_t id = idcnt++;
        e_action action = static_cast<e_action>( _arg1 );
        Card cardtype = Card( static_cast<e_color>( _arg2 ),
                              static_cast<e_number>( _arg3 ) );
        CardRule cardrule = CardRule( id, action, cardtype, _arg4 );
        cardrules.insert( { id, std::make_shared<CardRule>( cardrule ) } );
        return id;
        break;
    }
    return 0;
}

void GameRules::activateRule( uint8_t id ) {}

void GameRules::deactivateRule( uint8_t id ) {}

bool GameRules::canPlace( Card _stacktopcard, Card _playerplacecard,
                          bool _playerDraw )
{
    // Player can ONLY lay draw cards
    if ( _playerDraw )
    {
        CardRule cardrule = getCardRule( _playerplacecard );
        if ( cardrule.action == A_DRAW || cardrule.action == A_WILD_FOUR )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        // player can always place black cards
        if ( _playerplacecard.color == C_BLACK )
        {
            return true;
        }
        // player can place cards with same color or number
        if ( _stacktopcard.color == _playerplacecard.color ||
             _stacktopcard.number == _playerplacecard.number )
        {
            return true;
        }
        else
        {
            log->info( "cant place card!" );
            return false;
        }
    }
}

uint16_t GameRules::canDraw( uint16_t _curDrawAmount, uint8_t _cardsOnHand )
{
    // Do this many cards fit on his hand?
    if ( ( _cardsOnHand + _curDrawAmount ) > gameoptions.at( "maxDraw" ) )
    {
        // They don't? Then draw as many as you can!
        return ( gameoptions.at( "maxDraw" ) - _cardsOnHand );
    }
    else
    {
        // They do? So draw them!
        return _curDrawAmount;
    }
}

bool GameRules::canDraw( uint8_t _cardsOnHand )
{
    // check player's hand and maxCardsOnHand
    if ( ( _cardsOnHand + 1 ) > gameoptions.at( "maxDraw" ) )
    {
        return false;
    }
    else
    {
        return true;
    }
}

bool GameRules::checkSpecial( ) { return true; }

bool GameRules::setOption( std::string _option, uint8_t _value )
{
    gameoptions.at( _option ) = _value;
    return 1;
}

uint8_t GameRules::getOption( std::string _option )
{
    return gameoptions.at( _option );
}

CardRule GameRules::getCardRule( Card _card )
{
    for ( auto it = cardrules.begin( ); it != cardrules.end( ); it++ )
    {
        if ( it->second->cardtype == _card )
        { // if card matches one card definition in cardrules
            return *cardrules.at( it->second->ID );
        }
    }
    // if not found, card is regular(creates dummy rule)
    return CardRule( 0, A_REGULAR, Card( ), 0 );
}

bool GameRules::checkMaxCardsOnHand( uint8_t _cardsdrawn, uint8_t _cardsonhand )
{
    if ( ( _cardsonhand + _cardsdrawn ) > gameoptions.at( "maxCardsOnHand" ) )
    {
        return false;
    }
    else
    {
        return true;
    }
}
