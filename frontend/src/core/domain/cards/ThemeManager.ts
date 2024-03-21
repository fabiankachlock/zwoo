import { Awaiter } from '@/core/helper/Awaiter';
import Logger from '@/core/services/logging/logImport';

import { CardTheme, SerializedCardTheme } from './CardTheme';
import { CardThemeCache } from './CardThemeCache';
import { CardThemeIdentifier, CardThemeInformation, CardThemesMeta } from './CardThemeConfig';

export class CardThemeManager {
  static global: CardThemeManager;

  private themeCache: CardThemeCache<SerializedCardTheme>;

  private previewCache: CardThemeCache<CardTheme>;

  private meta: CardThemesMeta;

  private themesLoader: Promise<void> | undefined;

  public constructor(themeCache: CardThemeCache<SerializedCardTheme>, previewCache: CardThemeCache<CardTheme>) {
    this.themeCache = themeCache;
    this.previewCache = previewCache;
    this.meta = {
      themesList: [],
      themes: {},
      defaultTheme: {
        name: '',
        variant: '',
        version: ''
      }
    };
    const themeAwaiter = new Awaiter<void>();
    this.themesLoader = themeAwaiter.promise;
    Logger.Theme.log('manager is initializing');
    (async () => {
      this.meta = await this.loadThemes();
      themeAwaiter.callback();
      Logger.Theme.log('manager loaded config');
      Logger.Theme.debug(`found ${Object.keys(this.meta.themes).length} theme(s)`);
      this.themesLoader = undefined;
    })();
  }

  private createCacheKey(theme: CardThemeIdentifier, version: string): string {
    return `${theme.name}_${theme.variant}_v${version}`;
  }

  private async loadThemes(): Promise<CardThemesMeta> {
    try {
      const response = await fetch(`/assets/meta.json?t=${Date.now()}`).then(res => res.json());
      await this.themeCache.set('META', response);
      return response;
    } catch {
      Logger.Theme.log('falling back to cache');
      // TODO: fix cache...
      const response: CardThemesMeta = (await this.themeCache.get('META')) as unknown as CardThemesMeta;
      return (
        response || {
          defaultTheme: {
            name: '',
            variant: '',
            version: ''
          },
          themes: {},
          themesList: []
        }
      );
    }
  }

  public async getDefaultTheme(): Promise<CardThemeIdentifier> {
    await this.waitForThemes();
    return this.meta.defaultTheme;
  }

  public async loadTheme(theme: CardThemeIdentifier): Promise<CardTheme> {
    Logger.Theme.log(`loading theme ${theme.name}.${theme.variant}`);

    await this.waitForThemes();
    const cachedTheme = await this.themeCache.get(this.createCacheKey(theme, this.meta.themes[theme.name].config.version));
    if (cachedTheme) {
      // already loaded
      Logger.Theme.debug('theme already cached');
      return CardTheme.fromJson(cachedTheme);
    }

    if (!theme.name || !theme.variant) {
      Logger.Theme.warn('no theme selected');
      theme.name = this.meta.defaultTheme.name;
      theme.variant = this.meta.defaultTheme.variant;
    }

    const uri = this.meta.themes[theme.name].files[theme.variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    const loadedTheme = new CardTheme(theme.name, theme.variant, config, this.meta.themes[theme.name].config);
    this.themeCache.set(this.createCacheKey(theme, this.meta.themes[theme.name].config.version), loadedTheme.toJson());
    return loadedTheme;
  }

  public async loadPreview(theme: Omit<CardThemeIdentifier, 'version'>): Promise<CardTheme> {
    Logger.Theme.log(`loading preview for theme ${theme.name}.${theme.variant}`);

    const cachedTheme = await this.previewCache.get(this.createCacheKey(theme, 'PREVIEW'));
    if (cachedTheme) {
      // already loaded
      Logger.Theme.debug('theme preview already cached');
      return cachedTheme;
    }

    await this.waitForThemes();
    const uri = this.meta.themes[theme.name].files.previews[theme.variant];
    const config = await fetch(`/assets/${uri}`).then(res => res.json());
    const loadedPreview = new CardTheme(theme.name, theme.variant, config, this.meta.themes[theme.name].config);
    this.previewCache.set(this.createCacheKey(theme, 'PREVIEW'), loadedPreview);
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
    return this.meta.themesList
      .map(theme =>
        this.meta.themes[theme].config.variants.map(variant => ({
          name: theme,
          variant,
          version: this.meta.themes[theme].config.version
        }))
      )
      .flat();
  }

  public async getThemeInformation(theme: string): Promise<CardThemeInformation | undefined> {
    await this.waitForThemes();
    return this.meta.themes[theme].config;
  }

  public async getAllThemesInformation(): Promise<CardThemeInformation[]> {
    await this.waitForThemes();
    return this.meta.themesList.map(theme => this.meta.themes[theme].config);
  }
}
