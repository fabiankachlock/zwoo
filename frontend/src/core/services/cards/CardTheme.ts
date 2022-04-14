import { Card } from '../game/card';
import { CardDescriptor, CardThemeData, CardThemeIdentifier, CardThemeInformation, DefaultCardPreviews, MAX_THEME_PREVIEWS } from './CardThemeConfig';

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
    if (this.config.previews.length === 0) {
      return DefaultCardPreviews.slice();
    }
    return this.config.previews.slice(0, MAX_THEME_PREVIEWS - 1);
  }

  public getCard(card: Card | CardDescriptor): string {
    if (typeof card === 'string') {
      return this.data[card];
    }
    return this.data[this.cardToURI(card)] ?? '';
  }

  private cardToURI(card: Card): string {
    return `front_${card.color}_${card.type.toString(16)}`;
  }
}
