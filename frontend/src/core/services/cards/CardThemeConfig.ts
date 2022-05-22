export interface CardThemesMeta {
  themes: string[];
  variants: Record<string, string[]>;
  files: Record<string, Record<string, string>> & { previews: Record<string, Record<string, string>> };
  configs: Record<string, CardThemeInformation>;
}

export type CardThemeInformation = {
  name: string;
  description: string;
  author: string;
  isMultiLayer: boolean;
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

export const MAX_THEME_PREVIEWS = 6;

export const CardLayerWildcard = '$';
export const CardLayerSeparator = '_';

export type CardImageData = {
  layers: string[];
  description: string;
};
