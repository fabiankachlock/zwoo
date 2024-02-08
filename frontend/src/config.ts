export const AppConfig = Object.freeze({
  I18nLocale: import.meta.env.VUE_APP_I18N_LOCALE,
  I18nFallbackLocale: import.meta.env.VUE_APP_I18N_FALLBACK_LOCALE,
  IsBeta: import.meta.env.VUE_APP_BETA === 'true',
  Version: import.meta.env.VUE_APP_VERSION,
  VersionHash: import.meta.env.VUE_APP_VERSION_HASH,
  UseBackend: import.meta.env.VUE_APP_USE_BACKEND === 'true',
  DefaultEnv: import.meta.env.VUE_APP_DEFAULT_ENV,
  IsDev: import.meta.env.VUE_APP_DEVELOPMENT === 'true',
  ApiUrl: import.meta.env.VUE_APP_BACKEND,
  WsUrl: import.meta.env.VUE_APP_WS_OVERRIDE,
  Domain: import.meta.env.VUE_APP_DOMAIN,
  LogRushServer: import.meta.env.VUE_APP_LOG_RUSH_SERVER,
  VersionOverride: import.meta.env.VUE_APP_VERSION_OVERRIDE,
  IsTauri: import.meta.env.VUE_APP_IS_TAURI === 'true'
});
