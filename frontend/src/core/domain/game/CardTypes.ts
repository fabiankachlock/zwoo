export enum CardColor {
  none = 0,
  red = 1,
  green = 2,
  yellow = 3,
  blue = 4,
  black = 5
}

export enum CardType {
  none = 0,
  zero = 1,
  one = 2,
  two = 3,
  three = 4,
  four = 5,
  five = 6,
  six = 7,
  seven = 8,
  eight = 9,
  nine = 10,
  skip = 11,
  reverse = 12,
  draw_two = 13,
  wild = 14,
  wild_four = 15
}

export type Card = {
  type: CardType;
  color: CardColor;
};
