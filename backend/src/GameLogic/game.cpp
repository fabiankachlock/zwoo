#include <string>
#include <unordered_map>
#include <vector>

#include "GameLogic/game.h"

Game::Game( )
    : ID( 1 ), rules( GameRules( ) ), pile( CardPile( ) ), playerorder( ),
      draw_active( false ), current_player( playerorder.first( ) ),
      current_draw_amount( 0 ), active( false ), direction( 1 ),
      pick_active( false ), turncount( 0 )
{
    rules.createRule( 2, A_WILD, C_BLACK, N_WILD, 0 );           // DEFAULT RULE
    rules.createRule( 2, A_WILD_FOUR, C_BLACK, N_WILD_FOUR, 4 ); // DEFAULT RULE
    rules.createRule( 2, A_SKIP, C_NONE, N_SKIP, 0 );            // DEFAULT RULE
    rules.createRule( 2, A_REVERSE, C_NONE, N_REVERSE, 0 );      // DEFAULT RULE
    rules.createRule( 2, A_DRAW, C_NONE, N_DRAW_TWO, 2 );        // DEFAULT RULE
    rules.createRule( 2, A_NONE, C_NONE, N_NONE, 0 );            // DEFAULT RULE
}

Game::~Game( ) { stop( ); }

void Game::drawCard( std::shared_ptr<Player> _player )
{
    // draw card
    _player->addCard( this->pile.drawTopCard( ) );
}

void Game::drawCards( std::shared_ptr<Player> _player, uint8_t _amount )
{
    // draw cards
    for ( int i = 0; i < _amount; i++ )
    {
        _player->addCard( this->pile.drawTopCard( ) );
    }
}

void Game::placeCard( std::shared_ptr<Player> _player, Card _card )
{
    stack.addCard( _player->removeCard( _card ) );
}

void Game::placeCard( std::shared_ptr<Player> _player, Card _card1,
                      Card _card2 )
{
    std::shared_ptr<Card> card =
        _player->removeCard( _card1 ); // remove card from player inventory
    // modify card to new color/number
    card->color = _card2.color;
    card->number = _card2.number;
    stack.addCard( card ); // add card to stack
}

void Game::nextTurn( )
{
    // check if game has been won

    // check direction
    if ( direction == true )
    {
        current_player.operator++( );
    }
    else
    {
        current_player.operator--( );
    }
    turncount++;
}

bool Game::reset( )
{
    draw_active = false;
    current_draw_amount = 0;
    direction = 1;

    // reset players
    for ( auto it = players.begin( ); it != players.end( ); it++ )
    {
        if ( !it->second->reset( ) )
        {
            return false;
        }
    }

    // reset stack
    stack.reset( );

    // reset pile
    pile.reset( );

    return true;
}

bool Game::isValidAction( e_gaction _action )
{
    // check if issued action is expected
    for ( auto it = expected_actions.begin( ); it != expected_actions.end( );
          it++ )
    {
        if ( _action == *it )
        {
            return true;
        }
    }
    std::cout << "Invalid Action!!";
    return false;
}

bool Game::isWon( )
{
    // check all players for empty inventory
    for ( auto it = players.begin( ); it != players.end( ); it++ )
    {
        if ( it->second->IsEmpty( ) )
        {
            return true;
        }
    }

    return false;
}

bool Game::start( )
{

    // don't start if the game is already running
    if ( active )
    {
        return false;
    }

    // don't start if there is no/one player
    if ( players.size( ) <= 1 )
    {
        return false;
    }

    // init cardstack
    stack = CardStack( pile.drawTopCard( ) );

    // give each player initialCards amount of cards
    // add each player to the playorder
    for ( auto it = players.begin( ); it != players.end( ); it++ )
    {
        for ( int i = 0; i < rules.getOption( "initialCards" ); i++ )
        {
            it->second->addCard( pile.drawTopCard( ) );
        }
        this->playerorder.push_back( it->second ); // add player to order
    }
    current_player = playerorder.first( );

    // activate game
    active = true;

    expected_actions = { G_PLAYER_DRAW, G_PLAYER_PLACE };

    return true;
}

bool Game::stop( )
{
    expected_actions = { };
    active = false;

    reset( );

    // function call the game has ended
    std::cout << "GAME HAS BEEN STOPPED";

    return true;
}

bool Game::isActive( ) { return active; }

uint8_t Game::getPlayerCount( ) { return players.size( ); }

uint32_t Game::getID( ) { return ID; }

std::vector<uint32_t> Game::getPlayerIDs( )
{
    std::vector<uint32_t> playerids;
    for ( auto it = players.begin( ); it != players.end( ); it++ )
    {
        playerids.push_back( it->second->getID( ) );
    }
    return playerids;
}

std::shared_ptr<Player> Game::getPlayer( uint32_t _PUID )
{
    return players.at( _PUID );
}

std::shared_ptr<Player> Game::getCurPlayer( ) { return *current_player; }

std::vector<e_gaction> Game::getExpectedActions( ) { return expected_actions; }

void Game::placeCardEvent( Card _card )
{

    // local variables
    Card stacktopcard = stack.getTopCard( );
    CardRule cardrule = rules.getCardRule( _card );
    std::shared_ptr<Player> current_player = getCurPlayer( );

    // can player place the card (does he have it in his inv, etc.)

    // can the card be placed in general
    if ( rules.canPlace( stacktopcard, _card, draw_active ) &&
         current_player->hasCard( _card ) )
    {

        switch ( cardrule.action )
        {
        case A_REGULAR:
            // place the card on the stack
            placeCard( current_player, _card );

            // assign expected actions
            expected_actions = { G_PLAYER_PLACE, G_PLAYER_DRAW };

            nextTurn( );
            break;

        case A_SKIP:

            // place the card on the stack
            placeCard( current_player, _card );

            // trigger skip event
            skipEvent( );

            // assign expected actions
            expected_actions = { G_PLAYER_PLACE, G_PLAYER_DRAW };

            nextTurn( );
            break;

        case A_REVERSE:
            // place the card on the stack
            placeCard( current_player, _card );

            // trigger reverse event
            reverseEvent( );

            // assign expected actions
            expected_actions = { G_PLAYER_PLACE, G_PLAYER_DRAW };

            nextTurn( );
            break;

        case A_DRAW:
            draw_active = true;
            placeCard( current_player, _card );
            current_draw_amount += cardrule.mod;

            // assign expected actions
            expected_actions = { G_PLAYER_PLACE, G_PLAYER_DRAW };

            nextTurn( );
            break;

        case A_WILD:
            pick_active = true;
            tmpCard = _card;

            // assign expected actions
            expected_actions = { G_PLAYER_PICK };
            break;

        case A_WILD_FOUR:
            pick_active = true;
            draw_active = true;
            tmpCard = _card;
            current_draw_amount += cardrule.mod;

            // assign expected actions
            expected_actions = { G_PLAYER_PICK };
            break;

        case A_NONE:
            break;
        default:
            break;
        }

        if ( isWon( ) )
        {
            stop( );
            return;
        }
    }
    else
    {
        std::cout << "You cannot place this card!";
    }
}

void Game::drawCardEvent( )
{

    if ( draw_active )
    {
        // How much can be drawn at max
        current_draw_amount = rules.canDraw(
            current_draw_amount, ( *current_player )->getCardsOnHand( ) );

        std::cout << "drawn " << current_draw_amount << " cards!";
        drawCards( *current_player, current_draw_amount );
        draw_active = false;     // disable drawchain
        current_draw_amount = 0; // reset drawCount
        nextTurn( );
    }
    else
    {
        drawCard( *current_player );
        nextTurn( );
    }
    return;
}

void Game::skipEvent( ) { nextTurn( ); }

void Game::reverseEvent( ) { direction = !direction; }

void Game::wildEvent( uint32_t _chosencolor )
{
    Card invcard = tmpCard; // actual card that is in player's inventory
    Card placecard = Card(
        _chosencolor,
        invcard.number ); // card that appears on the stack e.g. red wild card
    placeCard( *current_player, invcard, placecard ); // place onto stack

    if ( draw_active == true )
    {
        expected_actions = { G_PLAYER_PLACE, G_PLAYER_DRAW };
    }
    nextTurn( );
}

bool Game::setOption( std::string _option, uint8_t _value )
{
    return rules.setOption( _option, _value );
}

uint8_t Game::getOption( std::string _option )
{
    return rules.getOption( _option );
}

void Game::PRINTSTATSPILE( ) { pile.PRINTSTATS( ); }

void Game::PRINTSTATSSTACK( ) { stack.PRINTSTATS( ); }

uint32_t Game::addPlayer( uint32_t puid )
{
    // check if game is active
    if ( !active )
    {

        // check if another player fits into game
        if ( !( ( players.size( ) + 1 ) > rules.getOption( "maxPlayers" ) ) )
        {

            std::shared_ptr<Player> player =
                std::make_shared<Player>( Player( puid ) ); // create a player
            uint32_t playerid = player->getID( );

            this->players.insert(
                { playerid, player } ); // insert player into this players-map
            return playerid;
        }
        else
        {
            std::cout << "THE LOBBY IS FREAKING FULL! WHAT THE HELL? ARE YOU "
                         "GOING TO SQUISH HIM INSIDE OR WHAT?";
            return -1;
        }
    }
    else
    {
        std::cout << "THE GAME IS ALREADY RUNNING YOU CUNT!";
        return -1;
    }
}

bool Game::removePlayer( uint32_t _playerid )
{
    this->players.erase( _playerid ); // remove player from players map
    // remove player from playerorder
    return true;
}
