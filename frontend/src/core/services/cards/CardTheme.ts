import { Card } from '../game/card';
import {
  CardDescriptor,
  CardImageData,
  CardLayerWildcard,
  CardThemeData,
  CardThemeIdentifier,
  CardThemeInformation,
  MAX_THEME_PREVIEWS
} from './CardThemeConfig';

export class CardTheme {
  constructor(
    public readonly name: string,
    public readonly variant: string,
    private readonly data: CardThemeData,
    private readonly config: CardThemeInformation
  ) {}

  get info(): CardThemeInformation {
    return this.config;
  }

  get identifier(): CardThemeIdentifier {
    return {
      name: this.name,
      variant: this.variant
    };
  }

  get previewCards(): string[] {
    return this.config.previews.slice(0, MAX_THEME_PREVIEWS - 1);
  }

  public getCard(card: Card | CardDescriptor | string): CardImageData {
    const layers: string[] = [];
    if (typeof card === 'string') {
      layers.push(card);
    } else {
      layers.push(...this.cardToURI(card));
    }
    return {
      layers: layers.map(identifier => this.data[identifier] ?? ''),
      description: typeof card === 'string' ? card : this.cardToAbsoluteUri(card)
    };
  }

  private cardToAbsoluteUri(card: Card): string {
    return `front_${card.color}_${card.type.toString(16)}`;
  }

  private cardToURI(card: Card): string[] {
    if (this.config.isMultiLayer) {
      return [`front_${card.color}_${CardLayerWildcard}`, `front_${CardLayerWildcard}_${card.type.toString(16)}`];
    }
    return [`front_${card.color}_${card.type.toString(16)}`];
  }
}
