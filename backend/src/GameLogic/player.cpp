#include "GameLogic/player.h"
#include "GameLogic/gamemanager.h"
#include "GameLogic/game.h"

Player::Player(uint32_t id) : ID(id), isEmpty(true) {
    cards.clear();
}

Player::~Player() {
    // release ID
}

void Player::update() {

    // check if player's hand is empty
    if (cards.size() <= 0) {
        isEmpty = true;
    }
    else {
        isEmpty = false;
    }
}

uint8_t Player::getCardsOnHand() {
    return cards.size();
}

bool Player::hasCard(Card _card) {
    for (auto it = cards.begin(); it < cards.end(); it++) {
        // compare color and number of cards
        if ((it->get()->color == _card.color) && (it->get()->number == _card.number)) {
            return true;
        }
    }
    return false;
}

bool Player::reset() {
    cards.clear();
    update();
    return 1;
}

void Player::addCard(std::shared_ptr<Card> _card) {
	cards.push_back(_card);
    update();
}

std::shared_ptr<Card> Player::removeCard(Card _card) {
    // go through each card in players inventory
    for (auto it = cards.begin(); it < cards.end(); it++) {
        // compare color and number of cards
        if ((it->get()->color == _card.color) && (it->get()->number == _card.number)) {
            std::shared_ptr<Card> card = *it; // create ptr of card
            cards.erase(it); // remove card from player
            update();
            return card;
        }
    }
    // DEBUG
    std::cout << "NO CARD FOUND IN PLAYER INVENTORY!";
    // DEBUG

    // NO CARD FOUND
    return nullptr;
}

uint32_t Player::getID() { 
	return this->ID; 
}

bool Player::IsEmpty() {
    return isEmpty;
}

void Player::PRINTSTATS() {

    std::cout << "==== PLAYER " << this->getID() << " ====\n";

    // print cards
    for (auto it = cards.begin(); it != cards.end(); it++) {
        std::cout << "[" << std::to_string(it->get()->color) << "," << std::to_string(it->get()->number) << "], ";
    }
    std::cout << "\nCards: " << std::to_string(getCardsOnHand()) << "\n";
}