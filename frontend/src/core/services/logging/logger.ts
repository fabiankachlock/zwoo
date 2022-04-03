import { LogEntry, BaseLogger, LogStore } from './logTypes';

export async function GetLogger(store: LogStore): Promise<() => BaseLogger> {
  const MAX_BUFFER_SIZE = 30;

  function LoggerFactory() {
    const _Logger = {
      buffer: [] as LogEntry[],
      _pushBuffer(msg: string) {
        this.buffer.push({
          date: Date.now(),
          log: msg
        });
        if (this.buffer.length >= MAX_BUFFER_SIZE) {
          store.addLogs(this.buffer);
          this.buffer = [];
        }
      },
      log(msg: string) {
        this._pushBuffer(`${msg}`);
      },
      info(msg: string) {
        this._pushBuffer(`[info] ${msg}`);
      },
      debug(msg: string) {
        this._pushBuffer(`[debug] ${msg}`);
      },
      warn(msg: string) {
        this._pushBuffer(`[warn] ${msg}`);
      },
      error(msg: string) {
        this._pushBuffer(`[ERROR] ${msg}`);
      },
      trace(error: Error, msg: string) {
        this._pushBuffer(`[trace] ${msg} ${error.stack}`);
      }
    };
    return _Logger;
  }

  return LoggerFactory;
}
