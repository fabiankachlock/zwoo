import { BaseLogger } from './logTypes';

export async function GetLogger(): Promise<(...loggers: BaseLogger[]) => BaseLogger> {
  function LoggerFactory(...loggers: BaseLogger[]) {
    const _Logger = {
      log(msg: string) {
        for (const logger of loggers) {
          logger.log(`${msg}`);
        }
      },
      info(msg: string) {
        for (const logger of loggers) {
          logger.info(msg);
        }
      },
      debug(msg: string) {
        for (const logger of loggers) {
          logger.debug(msg);
        }
      },
      warn(msg: string) {
        for (const logger of loggers) {
          logger.warn(msg);
        }
      },
      error(msg: string) {
        for (const logger of loggers) {
          logger.error(msg);
        }
      },
      trace(error: Error, msg: string) {
        for (const logger of loggers) {
          logger.trace(error, msg);
        }
      }
    };
    return _Logger;
  }

  return LoggerFactory;
}
