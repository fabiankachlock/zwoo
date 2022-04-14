export interface CardThemesMeta {
  themes: string[];
  variants: Record<string, string[]>;
  files: Record<string, Record<string, string>>;
  configs: Record<string, CardThemeInformation>;
}

export type CardThemeInformation = {
  name: string;
  description: string;
  author: string;
  isMultilayer: boolean;
  variants: string[];
  previews: string[];
};

export type CardThemeIdentifier = {
  name: string;
  variant: string;
};

export type CardThemeData = Record<string, string>;

export const CARD_THEME_VARIANT_AUTO = '@auto';

export const colorModeToVariant = (useDarkMode: boolean) => (useDarkMode ? 'dark' : 'light');

export enum CardDescriptor {
  BackUpright = 'back_u',
  BackSideways = 'back_s'
}

// TODO: ?
export const DefaultCardPreviews = [CardDescriptor.BackUpright, 'front_1_1', 'front_2_a', 'front_3_b', 'front_4_d', 'front_5_e'] as const;
