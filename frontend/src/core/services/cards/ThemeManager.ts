import { Awaiter } from '../helper/Awaiter';
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
      files: {}
    };
    const themeAwaiter = new Awaiter<void>();
    this.themesLoader = themeAwaiter.promise;
    (async () => {
      this.meta = await this.loadThemes();
      themeAwaiter.callback();
      this.themesLoader = undefined;
    })();
  }

  private async loadThemes(): Promise<CardThemesMeta> {
    return fetch('/assets/meta.json').then(res => res.json());
  }

  public async loadTheme(theme: string, variant: string): Promise<CardTheme> {
    await this.waitForThemes();
    const uri = this.meta.files[theme][variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    return new CardTheme(theme, variant, config);
  }

  private waitForThemes(): Promise<void> {
    if (this.themesLoader) {
      return this.themesLoader;
    }
    return Promise.resolve();
  }
}
