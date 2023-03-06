export interface WriteableMessageSource<T> {
  writeMessage(message: T): void;
}

export interface ReadableMessageSource<T> {
  readMessages(handler: (message: T) => void): void;
}

export interface BidirectionalMessageSource<T> extends WriteableMessageSource<T>, ReadableMessageSource<T> {}

export abstract class RealtimeGameMessageAdapter {
  abstract state: number;
  abstract onMessage(handler: (msg: string) => void): void;
  abstract sendMessage(msg: string): void;
  abstract close(): void;
}
