/* eslint-disable */
import { BaseLogger, LoggerInterface } from './logTypes';

export const Logger: LoggerInterface & { name: string } = {
  name: '',
  createOne(_name): LoggerInterface & { name: string } {
    return { ...Logger, name: _name ?? 'Global' };
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

let LoggerBase: BaseLogger = {
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
  const loggerFactory = await loggerModule.GetLogger(store);

  LoggerBase = loggerFactory();
});
