/// <reference types="vite/client" />

interface ImportMetaEnv {
	readonly VUE_APP_I18N_LOCALE: string;
	readonly VUE_APP_I18N_FALLBACK_LOCALE: string;
	readonly VUE_APP_BETA: string;
	readonly VUE_APP_VERSION: string;
	readonly VUE_APP_VERSION_HASH: string;
	readonly VUE_APP_USE_BACKEND: string;
	readonly VUE_APP_DEVELOPMENT: string;
	readonly VUE_APP_BACKEND: string;
	readonly VUE_APP_WS_OVERRIDE: string;
	readonly VUE_APP_DOMAIN: string;
	readonly VUE_APP_LOG_RUSH_SERVER: string;
	readonly VUE_APP_VERSION_OVERRIDE: string | undefined;
	readonly VUE_APP_IS_TAURI: string | undefined;
}

interface ImportMeta {
	readonly env: ImportMetaEnv;
}
