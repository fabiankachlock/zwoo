import { Card, CardColor } from '../game/CardTypes';
import {
  CardDescriptor,
  CardImageData,
  CardLayerSeparator,
  CardLayerWildcard,
  CardThemeData,
  CardThemeIdentifier,
  CardThemeInformation,
  MAX_THEME_PREVIEWS
} from './CardThemeConfig';

export type SerializedCardTheme = {
  name: string;
  variant: string;
  data: CardThemeData;
  config: CardThemeInformation;
};

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

  get colors(): Record<CardColor, string> {
    return this.config.colors[this.variant];
  }

  public getCard(card: Card | CardDescriptor | string): CardImageData {
    const layers: string[] = [];
    if (Object.values(CardDescriptor).includes(card as CardDescriptor)) {
      layers.push(card as string);
    } else if (typeof card === 'string' && this.config.isMultiLayer) {
      layers.push(...this.cardDescriptionToLayers(card));
    } else if (typeof card === 'string') {
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

  private cardDescriptionToLayers(descriptor: string): string[] {
    const firstLayer = descriptor.replace(new RegExp(CardLayerSeparator + '.$'), CardLayerSeparator + CardLayerWildcard);
    const secondLayer = descriptor.replace(
      new RegExp(CardLayerSeparator + '.' + CardLayerSeparator),
      CardLayerSeparator + CardLayerWildcard + CardLayerSeparator
    );
    return [firstLayer, secondLayer];
  }

  public toJson(): SerializedCardTheme {
    return {
      name: this.name,
      variant: this.variant,
      config: this.config,
      data: this.data
    };
  }

  public static fromJson(data: SerializedCardTheme): CardTheme {
    return new CardTheme(data.name, data.variant, data.data, data.config);
  }
}
