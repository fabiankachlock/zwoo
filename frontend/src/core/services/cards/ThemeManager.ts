import { Awaiter } from '../helper/Awaiter';
import Logger from '../logging/logImport';
import { CardTheme } from './CardTheme';
import { CardThemesMeta } from './CardThemeConfig';

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

  public async loadTheme(theme: string, variant: string): Promise<CardTheme> {
    Logger.Theme.log(`loading theme ${theme}.${variant}`);
    await this.waitForThemes();
    const uri = this.meta.files[theme][variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    return new CardTheme(theme, variant, config);
  }

  private waitForThemes(): Promise<void> {
    if (this.themesLoader) {
      Logger.Theme.debug('waiting for config to be loaded');
      return this.themesLoader;
    }
    return Promise.resolve();
  }
}
