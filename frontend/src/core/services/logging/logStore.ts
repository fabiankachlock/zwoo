import { DexieConstructor, LogEntry, LogStore } from './logTypes';

export async function GetLogStore(): Promise<LogStore> {
  const { Dexie } = (await import(/* webpackChunkName: "logging" */ 'dexie')) as unknown as { Dexie: DexieConstructor };

  const DBName = 'zwoo';
  const TableName = 'logs';
  const db = new Dexie(DBName);

  const LogStoreImpl = {
    isReady: false,
    construct() {
      db.version(1).stores({
        [TableName]: 'date, log'
      });
      db.on('ready', () => {
        this.isReady = true;
        console.info('log store ready');
        this._readyHandler();
      });
      db.open();
    },
    async readAll(): Promise<LogEntry[]> {
      return db.table(TableName).toArray();
    },
    async addLogs(logs: LogEntry[]) {
      await db.table(TableName).bulkAdd(logs);
    },
    async clear() {
      return db.table(TableName).clear();
    },
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    _readyHandler: () => {},
    onReady(handler: () => void) {
      this._readyHandler = handler;
    }
  };

  LogStoreImpl.construct();
  return LogStoreImpl;
}
