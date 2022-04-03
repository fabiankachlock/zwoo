/* eslint-disable */
import { BaseLogger, ExtendedLogger, LoggerInterface, ZwooLogger } from './logTypes';

export const Logger: ZwooLogger = {
  name: 'global',
  createOne(_name): LoggerInterface & { name: string } {
    return { ...Logger, name: _name ?? this.name };
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
  },
  Websocket: (this as unknown as LoggerInterface).createOne('ws'),
  Api: (this as unknown as LoggerInterface).createOne('api'),
  Zrp: (this as unknown as LoggerInterface).createOne('zrp')
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

import(/* webpackChunkName: "logging" */ './logStore').then(async storeModule => {
  const loggerModule = await import(/* webpackChunkName: "logging" */ './logger');

  const store = await storeModule.GetLogStore();
  store.onReady(() => {
    if (LoggerBase.initialized) {
      LoggerBase.debug('log store ready');
    } else {
      store.addLogs([
        {
          date: Date.now(),
          log: 'log stored ready'
        }
      ]);
    }
  });
  const loggerFactory = await loggerModule.GetLogger(store);

  LoggerBase = {
    ...loggerFactory(),
    initialized: true
  };
  LoggerBase.debug('logger started');
});
