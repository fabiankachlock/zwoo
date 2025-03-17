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
import { CardThemeManager } from './ThemeManager';

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

  public async getCard(card: Card | CardDescriptor | string): Promise<CardImageData> {
    const layers: string[] = [];
    if (Object.values(CardDescriptor).includes(card as CardDescriptor)) {
      layers.push(card as string);
    } else if (typeof card === 'string' && this.config.isMultiLayer) {
      layers.push(...this.cardDescriptionToLayers(card));
    } else if (typeof card === 'string') {
      layers.push(card);
    } else {
      layers.push(...this.cardToLayers(card));
    }

    const cardData = {
      layers: layers.map(identifier => this.data[identifier] ?? ''),
      description: typeof card === 'string' ? card : this.cardToDescription(card)
    };

    if (cardData.layers.some(layer => !layer)) {
      const defaultTheme = await this.getDefaultTheme();
      return defaultTheme.getCard(card);
    }

    return cardData;
  }

  private cardToDescription(card: Card): string {
    return `front_${card.color}_${card.type.toString(36)}`;
  }

  private cardToLayers(card: Card): string[] {
    const descriptor = this.cardToDescription(card);
    if (this.config.isMultiLayer && !this.config.customCards?.includes(descriptor)) {
      return [`front_${card.color}_${CardLayerWildcard}`, `front_${CardLayerWildcard}_${card.type.toString(36)}`];
    }
    return [descriptor];
  }

  private cardDescriptionToLayers(descriptor: string): string[] {
    // If the descriptor is a custom card in a multi layer theme, return it as is
    if (this.config.isMultiLayer && this.config.customCards?.includes(descriptor)) {
      return [descriptor];
    }

    const firstLayer = descriptor.replace(new RegExp(CardLayerSeparator + '.$'), CardLayerSeparator + CardLayerWildcard);
    const secondLayer = descriptor.replace(
      new RegExp(CardLayerSeparator + '.' + CardLayerSeparator),
      CardLayerSeparator + CardLayerWildcard + CardLayerSeparator
    );
    return [firstLayer, secondLayer];
  }

  private async getDefaultTheme(): Promise<CardTheme> {
    const defaultThemeIdentifier = await CardThemeManager.global.getDefaultTheme();
    return CardThemeManager.global.loadTheme(defaultThemeIdentifier);
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
