/* eslint-disable */
import { BaseLogger, ExtendedLogger, LogEntry, LoggerInterface, ZwooLogger } from './logTypes';

let _Logger: ExtendedLogger = {
  name: 'global',
  createOne(_name): LoggerInterface & { name: string } {
    return { ..._Logger, name: _name ?? this.name };
  },
  info(msg) {
    LoggerBase.info(`[${this.name}] ${msg}`);
  },
  log(msg) {
    LoggerBase.log(`[${this.name}] ${msg}`);
  },
  debug(msg) {
    LoggerBase.debug(`[${this.name}] ${msg}`);
  },
  warn(msg) {
    LoggerBase.warn(`[${this.name}] ${msg}`);
  },
  error(msg) {
    LoggerBase.error(`[${this.name}] ${msg}`);
  },
  trace(error, msg) {
    LoggerBase.trace(error, `[${this.name}] ${msg}`);
  }
};

let LoggerBase: BaseLogger & { initialized: boolean } = {
  initialized: false,
  info() {},
  log() {},
  debug() {},
  warn() {},
  error() {},
  trace() {}
};

let StoreRef = {
  resetStore: () => {},
  getAll: () => Promise.resolve([] as LogEntry[])
};

const setupLogger = (mode: string | null) => {
  import(/* webpackChunkName: "logging" */ './logStore').then(async storeModule => {
    const storeLoggerModule = await import(/* webpackChunkName: "logging" */ './storeLogger');
    const logRushLoggerModule = await import(/* webpackChunkName: "logging" */ './logRushLogger');
    const consoleLoggerModule = await import(/* webpackChunkName: "logging" */ './consoleLogger');
    const multiLoggerModule = await import(/* webpackChunkName: "logging" */ './multiLogger');

    const store = await storeModule.GetLogStore();
    StoreRef.resetStore = store.clear;
    StoreRef.getAll = store.readAll;
    store.onReady(() => {
      _Logger.debug('log store ready');
      _Logger.debug('logger started');
      _Logger.debug('--start-config--');
      _Logger.debug('VERSION: ' + process.env.VUE_APP_VERSION);
      _Logger.debug('VERSION_HASH: ' + process.env.VUE_APP_VERSION_HASH);
      _Logger.debug('DOMAIN: ' + process.env.VUE_APP_DOMAIN);
      _Logger.debug('USE_BACKEND: ' + process.env.VUE_APP_USE_BACKEND);
      _Logger.debug('BACKEND_URL: ' + process.env.VUE_APP_BACKEND_URL);
      _Logger.debug('WS_OVERRIDE: ' + process.env.VUE_APP_WS_OVERRIDE);
      _Logger.debug('I18N_LOCALE: ' + process.env.VUE_APP_I18N_LOCALE);
      _Logger.debug('I18N_FALLBACK_LOCALE: ' + process.env.VUE_APP_I18N_FALLBACK_LOCALE);
      _Logger.debug('--end-config--');
    });

    const loggers: (() => BaseLogger)[] = [];
    const multiFactory = await multiLoggerModule.GetLogger();
    mode = mode ?? '';

    if (mode.includes('s')) {
      loggers.push(await storeLoggerModule.GetLogger(store));
    }
    if (mode.includes('c')) {
      loggers.push(await consoleLoggerModule.GetLogger());
    }
    if (mode.includes('l')) {
      loggers.push(await logRushLoggerModule.GetLogger());
    }

    LoggerBase = {
      ...multiFactory(...loggers.map(factory => factory())),
      initialized: true
    };

    _Logger.debug('logger loaded');
  });
};

setupLogger(localStorage.getItem('zwoo:logging'));

export const Logger: ZwooLogger = {
  ..._Logger,
  Websocket: _Logger.createOne('ws'),
  Api: _Logger.createOne('api'),
  Zrp: _Logger.createOne('zrp'),
  RouterGuard: _Logger.createOne('guard'),
  Theme: _Logger.createOne('theme')
};

export const LogStore = {
  reset: () => StoreRef.resetStore(),
  getAll: () => StoreRef.getAll()
};

export default Logger;
