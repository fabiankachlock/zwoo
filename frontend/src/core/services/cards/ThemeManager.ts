import { Awaiter } from '../helper/Awaiter';
import Logger from '../logging/logImport';
import { CardTheme } from './CardTheme';
import { CardThemeInformation, CardThemesMeta, CardThemeIdentifier } from './CardThemeConfig';

export class CardThemeManager {
  static global = new CardThemeManager();

  private meta: CardThemesMeta;

  private themesLoader: Promise<void> | undefined;

  private constructor() {
    this.meta = {
      themes: [],
      variants: {},
      files: {},
      configs: {}
    };
    const themeAwaiter = new Awaiter<void>();
    this.themesLoader = themeAwaiter.promise;
    Logger.Theme.log('manager is initializing');
    (async () => {
      this.meta = await this.loadThemes();
      themeAwaiter.callback();
      Logger.Theme.log('manager loaded config');
      Logger.Theme.debug(`found ${this.meta.themes.length} theme(s)`);
      this.themesLoader = undefined;
    })();
  }

  private async loadThemes(): Promise<CardThemesMeta> {
    return fetch('/assets/meta.json').then(res => res.json());
  }

  public async loadTheme(theme: CardThemeIdentifier): Promise<CardTheme> {
    Logger.Theme.log(`loading theme ${theme.name}.${theme.variant}`);
    await this.waitForThemes();
    const uri = this.meta.files[theme.name][theme.variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    return new CardTheme(theme.name, theme.variant, config, this.meta.configs[theme.name]);
  }

  private waitForThemes(): Promise<void> {
    if (this.themesLoader) {
      Logger.Theme.debug('waiting for config to be loaded');
      return this.themesLoader;
    }
    return Promise.resolve();
  }

  public async getSelectableThemes(): Promise<CardThemeIdentifier[]> {
    await this.waitForThemes();
    return this.meta.themes
      .map(theme =>
        this.meta.variants[theme].map(variant => ({
          name: theme,
          variant
        }))
      )
      .flat();
  }

  public async getThemeInformation(theme: string): Promise<CardThemeInformation | undefined> {
    await this.waitForThemes();
    return this.meta.configs[theme];
  }

  public async getAllThemesInformation(): Promise<CardThemeInformation[]> {
    await this.waitForThemes();
    return this.meta.themes.map(theme => this.meta.configs[theme]);
  }
}
