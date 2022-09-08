import { Awaiter } from '../helper/Awaiter';
import { QueuedCache } from '../helper/QueuedCache';
import Logger from '../logging/logImport';
import { CardTheme } from './CardTheme';
import { CardThemeInformation, CardThemesMeta, CardThemeIdentifier } from './CardThemeConfig';

export class CardThemeManager {
  static global = new CardThemeManager();

  private themeCache = new QueuedCache<CardTheme>(3);

  private previewCache = new QueuedCache<CardTheme>(Infinity);

  private meta: CardThemesMeta;

  private themesLoader: Promise<void> | undefined;

  private constructor() {
    this.meta = {
      themes: [],
      defaultTheme: {
        name: '',
        variant: ''
      },
      variants: {},
      files: {
        previews: {}
      },
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

  private createCacheKey(theme: CardThemeIdentifier): string {
    return `${theme.name}_${theme.variant}`;
  }

  private async loadThemes(): Promise<CardThemesMeta> {
    return fetch(`/assets/meta.json?t=${Date.now()}`).then(res => res.json());
  }

  public async getDefaultTheme(): Promise<CardThemeIdentifier> {
    await this.waitForThemes();
    return this.meta.defaultTheme;
  }

  public async loadTheme(theme: CardThemeIdentifier): Promise<CardTheme> {
    Logger.Theme.log(`loading theme ${theme.name}.${theme.variant}`);

    const cachedTheme = this.themeCache.get(this.createCacheKey(theme));
    if (cachedTheme) {
      // already loaded
      Logger.Theme.debug('theme already cached');
      return cachedTheme;
    }

    await this.waitForThemes();
    if (!theme.name || !theme.variant) {
      Logger.Theme.warn('no theme selected');
      theme.name = this.meta.defaultTheme.name;
      theme.variant = this.meta.defaultTheme.variant;
    }

    const uri = this.meta.files[theme.name][theme.variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    const loadedTheme = new CardTheme(theme.name, theme.variant, config, this.meta.configs[theme.name]);
    this.themeCache.set(this.createCacheKey(theme), loadedTheme);
    return loadedTheme;
  }

  public async loadPreview(theme: CardThemeIdentifier): Promise<CardTheme> {
    Logger.Theme.log(`loading preview for theme ${theme.name}.${theme.variant}`);

    const cachedTheme = this.previewCache.get(this.createCacheKey(theme));
    if (cachedTheme) {
      // already loaded
      Logger.Theme.debug('theme preview already cached');
      return cachedTheme;
    }

    await this.waitForThemes();
    const uri = this.meta.files.previews[theme.name][theme.variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    const loadedPreview = new CardTheme(theme.name, theme.variant, config, this.meta.configs[theme.name]);
    this.previewCache.set(this.createCacheKey(theme), loadedPreview);
    return loadedPreview;
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
