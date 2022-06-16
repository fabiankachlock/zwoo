#include "GameLogic/card.h"

// Card //

Card::Card( e_color _color, e_number _number )
{
    this->color = _color;
    this->number = _number;
}

Card::Card( uint8_t _color, uint8_t _number )
{
    this->color = static_cast<e_color>( _color );
    this->number = static_cast<e_number>( _number );
}

Card::Card( ) : color( C_NONE ), number( N_NONE ) {}

bool Card::operator==( Card &_card )
{
    if ( ( this->color == _card.color || this->color == C_NONE ) &&
         this->number == _card.number )
    {
        return true;
    }
    else
    {
        return false;
    }
}

// CardPile //

CardPile::CardPile( )
{
    // go through all 4 colors
    for ( int i = 1; i <= 4; i++ )
    {
        // create 1x zero
        createCard(
            Card( static_cast<e_color>( i ), static_cast<e_number>( 1 ) ) );
        // go through remaining cards(12) 2x
        for ( int x = 0; x <= 1; x++ )
        {
            // create cards from 1-9, reverse, skip, draw_two
            for ( int j = 2; j <= 13; j++ )
            {
                createCard( Card( static_cast<e_color>( i ),
                                  static_cast<e_number>( j ) ) );
            }
        }
    }
    // create black cards
    for ( int i = 0; i <= 3; i++ )
    {
        createCard(
            Card( static_cast<e_color>( 5 ), static_cast<e_number>( 14 ) ) );
        createCard(
            Card( static_cast<e_color>( 5 ), static_cast<e_number>( 15 ) ) );
    }
    shuffle( );
}

CardPile::~CardPile( ) {}

bool CardPile::isEmpty( )
{
    if ( cards.size( ) == 0 )
    {
        return true;
    }
    else
    {
        return false;
    }
}

void CardPile::shuffle( )
{
    std::random_device rd;
    std::default_random_engine rng( rd( ) );
    std::shuffle( cards.begin( ), cards.end( ), rng );
}

void CardPile::generate( )
{
    // go through all 4 colors
    for ( int i = 1; i <= 4; i++ )
    {
        // create 1x zero
        createCard(
            Card( static_cast<e_color>( i ), static_cast<e_number>( 1 ) ) );
        // go through remaining cards(12) 2x
        for ( int x = 0; x <= 1; x++ )
        {
            // create cards from 1-9, reverse, skip, draw_two
            for ( int j = 2; j <= 13; j++ )
            {
                createCard( Card( static_cast<e_color>( i ),
                                  static_cast<e_number>( j ) ) );
            }
        }
    }
    // create black cards
    for ( int i = 0; i <= 3; i++ )
    {
        createCard(
            Card( static_cast<e_color>( 5 ), static_cast<e_number>( 14 ) ) );
        createCard(
            Card( static_cast<e_color>( 5 ), static_cast<e_number>( 15 ) ) );
    }
    shuffle( );
}

bool CardPile::reset( )
{
    cards.clear( );
    generate( );
    return true;
}

void CardPile::createCard( Card _card )
{
    cards.push_back( std::make_shared<Card>(
        _card ) ); // insert card at last position(top of pile)
}

void CardPile::destroyCard( Card _card )
{
    // go through cardpile
    for ( int i = 0; i < cards.size( ); i++ )
    {
        // check color and number of card
        if ( ( cards.at( i )->color == _card.color ) &&
             ( cards.at( i )->number == _card.number ) )
        {
            // remove card from pile
            cards.erase( cards.begin( ) + i );
            return;
        }
    }
    // NO CARD FOUND IN PILE
}

std::shared_ptr<Card> CardPile::drawTopCard( )
{
    std::shared_ptr<Card> card =
        cards.at( cards.size( ) - 1 ); // gets last vector element
    cards.pop_back( );                 // destroys last element
    // reduceCurSize();
    return card;
}

void CardPile::addCard( std::shared_ptr<Card> _card )
{
    cards.push_back( _card ); // insert card at last position(top of pile)
}

void CardPile::addCards( std::vector<std::shared_ptr<Card>> _cards )
{
    cards.insert( cards.end( ), _cards.begin( ),
                  _cards.end( ) ); // append _cards to cardpile
}

void CardPile::insertCardStack( std::shared_ptr<CardStack> _stack )
{
    addCards( _stack->drawBackCards( ) );
}

void CardPile::PRINTSTATS( )
{

    log->info( "==== CARDPILE ====\nCards: {}", cards.size( ) );
}

// CardStack //

CardStack::CardStack( ) { cards.clear( ); }

CardStack::CardStack( std::shared_ptr<Card> _card )
{
    cards.push_back( _card );
}

void CardStack::addCard( std::shared_ptr<Card> _card )
{
    cards.push_back( _card );
}

Card CardStack::getTopCard( )
{
    Card topcard = *cards.at( cards.size( ) - 1 );
    return topcard;
}

bool CardStack::reset( )
{
    cards.clear( );
    return true;
}

std::vector<std::shared_ptr<Card>> CardStack::drawBackCards( )
{
    std::vector<std::shared_ptr<Card>> backcards =
        this->cards;       // create copy of all cards on stack
    backcards.pop_back( ); // remove last object(top card) from copy

    cards.erase( cards.begin( ),
                 cards.end( ) - 1 ); // remove all objects except last one(top
                                     // card) from actual stack

    return backcards;
}

void CardStack::PRINTSTATS( )
{
    log->info( "==== STACK ====\n" );

    // print cards
    for ( auto it = std::begin( cards ); it != std::end( cards ); it++ )
    {
        log->info( "[ {1}, {2}], ", std::to_string( it->get( )->color ),
                   std::to_string( it->get( )->number ) );
    }

    log->info( "\nCards: {}\n", cards.size( ) );
}