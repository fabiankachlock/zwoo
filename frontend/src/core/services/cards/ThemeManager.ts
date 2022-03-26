export class CardThemeManager {
  public static baseUrl = '/img/cards';

  public static getThemeDirectory = (themeName: string) => `${CardThemeManager.baseUrl}/${themeName}`;

  public static getCardBackUrl = (themeName: string, colorTheme: string) => `${CardThemeManager.getThemeDirectory(themeName)}/back/${colorTheme}.png`;
}
