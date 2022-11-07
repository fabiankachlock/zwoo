/* eslint-disable */
import { AppConfig } from '@/config';
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

let LoggerBase = {
  initialized: false,
  _qInfo: [] as unknown[][],
  info(...args: unknown[]) {
    this._qInfo.push(args);
  },

  _qLog: [] as unknown[][],
  log(...args: unknown[]) {
    this._qLog.push(args);
  },

  _qDebug: [] as unknown[][],
  debug(...args: unknown[]) {
    this._qDebug.push(args);
  },

  _qWarn: [] as unknown[][],
  warn(...args: unknown[]) {
    this._qWarn.push(args);
  },

  _qError: [] as unknown[][],
  error(...args: unknown[]) {
    this._qError.push(args);
  },

  _qTrace: [] as unknown[][],
  trace(...args: unknown[]) {
    this._qTrace.push(args);
  }
};

let StoreRef = {
  resetStore: () => {},
  getAll: () => Promise.resolve([] as LogEntry[])
};

const setupLogger = (mode: string | null) => {
  import('./logStore').then(async storeModule => {
    const storeLoggerModule = await import('./storeLogger');
    const logRushLoggerModule = await import('./logRushLogger');
    const consoleLoggerModule = await import('./consoleLogger');
    const multiLoggerModule = await import('./multiLogger');

    const store = await storeModule.GetLogStore();
    StoreRef.resetStore = store.clear;
    StoreRef.getAll = store.readAll;
    store.onReady(() => {
      _Logger.debug('log store ready');
      _Logger.debug('logger started');
      _Logger.debug('--start-config--');
      for (const [key, value] of Object.entries(AppConfig)) {
        _Logger.debug(`${key}=${value}`);
      }
      _Logger.debug('DEVICE_ID: ' + window.DEVICE_ID);
      _Logger.debug('NAVIGATOR_LNG: ' + navigator.language);
      _Logger.debug('USER_AGENT: ' + navigator.userAgent);
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
      ...LoggerBase,
      ...multiFactory(...loggers.map(factory => factory())),
      initialized: true
    };

    LoggerBase._qInfo.forEach(call => LoggerBase.info(...call));
    LoggerBase._qDebug.forEach(call => LoggerBase.debug(...call));
    LoggerBase._qLog.forEach(call => LoggerBase.log(...call));
    LoggerBase._qWarn.forEach(call => LoggerBase.warn(...call));
    LoggerBase._qError.forEach(call => LoggerBase.error(...call));
    LoggerBase._qTrace.forEach(call => LoggerBase.trace(...call));

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
