export type { DexieConstructor } from 'dexie';
export type LogEntry = { date: number; log: string };

export interface LogStore {
	isReady: boolean;
	readAll(): Promise<LogEntry[]>;
	addLogs(logs: LogEntry[]): Promise<void>;
	clear(): Promise<void>;
}

export interface BaseLogger {
	log(msg: string);
	info(msg: string);
	debug(msg: string);
	warn(msg: string);
	error(msg: string);
	trace(error: Error, msg: string);
}

export interface LoggerInterface extends BaseLogger {
	createOne(name?: string): BaseLogger;
}
