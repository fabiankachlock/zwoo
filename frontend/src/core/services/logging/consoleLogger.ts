import { BaseLogger } from './logTypes';

export async function GetLogger(): Promise<() => BaseLogger> {
  function LoggerFactory() {
    const _Logger = {
      log(msg: string) {
        console.log(`${msg}`);
      },
      info(msg: string) {
        console.info(`[info] ${msg}`);
      },
      debug(msg: string) {
        console.debug(`[debug] ${msg}`);
      },
      warn(msg: string) {
        console.warn(`[warn] ${msg}`);
      },
      error(msg: string) {
        console.error(`[ERROR] ${msg}`);
      },
      trace(error: Error, msg: string) {
        console.trace(`[trace] ${msg} ${error.stack}`);
      }
    };
    return _Logger;
  }

  return LoggerFactory;
}
