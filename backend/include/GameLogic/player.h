#pragma once
#include <memory>
#include <unordered_map>
#include <vector>

class Game;
struct Card;

class Player
{
  private:
    std::vector<std::shared_ptr<Card>> cards;
    const uint32_t ID;
    bool isEmpty; // Player has no cards

    void update( );

  public:
    Player( uint32_t );
    ~Player( );

    bool isActive( );
    uint8_t getCardsOnHand( );
    bool hasCard( Card _card );
    bool reset( );

    void addCard( std::shared_ptr<Card> _card );
    std::shared_ptr<Card>
    removeCard( Card _card ); // remove card from inventory

    // check rules and search/place card
    void placeCard( Card _card );

    uint32_t getID( );
    bool IsEmpty( );

    void PRINTSTATS( );
};