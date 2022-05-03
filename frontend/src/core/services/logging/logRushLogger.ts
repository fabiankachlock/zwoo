import { LogEntry, BaseLogger } from './logTypes';

export async function GetLogger(): Promise<() => BaseLogger> {
  const MAX_BUFFER_SIZE = 1;

  const req = await fetch('http://localhost:7000/stream/register', {
    method: 'post',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      alias: 'zwoo-frontend-test'
    })
  });

  if (req.status !== 200) {
    throw new Error('cant register log stream', await req.json());
  }

  const stream = (await req.json()) as {
    id: string;
    alias: string;
    key: string;
  };

  window.onbeforeunload = async () => {
    await fetch('http://localhost:7000/stream/unregister', {
      method: 'post',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        id: stream.id,
        key: stream.key
      })
    }).then(req => req.json());
  };

  function LoggerFactory() {
    const _Logger = {
      buffer: [] as LogEntry[],
      _saveBuffer() {
        fetch('http://localhost:7000/batch', {
          method: 'post',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            stream: stream.id,
            logs: this.buffer.map(l => ({
              log: l.log,
              timestamp: l.date
            }))
          })
        });
        this.buffer = [];
      },
      _pushBuffer(msg: string) {
        this.buffer.push({
          date: Date.now(),
          log: msg
        } as LogEntry);
        if (this.buffer.length >= MAX_BUFFER_SIZE) {
          this._saveBuffer();
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
