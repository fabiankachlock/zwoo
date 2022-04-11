export interface CardThemesMeta {
  themes: string[];
  variants: Record<string, string[]>;
  files: Record<string, Record<string, string>>;
}

export type CardThemeConfig = Record<string, string>;

export const CARD_THEME_VARIANT_AUTO = '@auto';

export const colorModeToVariant = (useDarkMode: boolean) => (useDarkMode ? 'dark' : 'light');

export enum CardDescriptor {
  BackUpright = 'back_u',
  BackSideways = 'back_s'
}
