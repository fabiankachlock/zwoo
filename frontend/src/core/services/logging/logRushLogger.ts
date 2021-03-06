import { BaseLogger } from './logTypes';
import { LogRushClient } from '@log-rush/client';

export async function GetLogger(): Promise<() => BaseLogger> {
  const MAX_BUFFER_SIZE = 10;

  const client = new LogRushClient({
    dataSourceUrl: process.env.VUE_APP_LOG_RUSH_SERVER ?? '',
    batchSize: MAX_BUFFER_SIZE
  });

  const sId = localStorage.getItem('zwoo:rmsid') ?? '';
  const sKey = localStorage.getItem('zwoo:rmskey') ?? '';

  const stream = await client.resumeStream(`zwoo-${window.DEVICE_ID ?? 'unknown user'}`, sId, sKey);

  localStorage.setItem('zwoo:rmsid', stream.id);
  localStorage.setItem('zwoo:rmskey', stream.secretKey);

  window.onbeforeunload = async () => {
    await client.disconnect();
  };

  function LoggerFactory() {
    const _Logger = {
      _pushLog(msg: string) {
        stream.log(msg);
      },
      log(msg: string) {
        this._pushLog(`${msg}`);
      },
      info(msg: string) {
        this._pushLog(`[info] ${msg}`);
      },
      debug(msg: string) {
        this._pushLog(`[debug] ${msg}`);
      },
      warn(msg: string) {
        this._pushLog(`[warn] ${msg}`);
      },
      error(msg: string) {
        this._pushLog(`[ERROR] ${msg}`);
      },
      trace(error: Error, msg: string) {
        this._pushLog(`[trace] ${msg} ${error.stack}`);
      }
    };
    return _Logger;
  }

  return LoggerFactory;
}
