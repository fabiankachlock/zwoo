export interface WriteableMessageSource<T> {
  writeMessage(message: T): void;
}

export interface ReadableMessageSource<T> {
  readMessages(handler: (message: T) => void): void;
}

export interface BidirectionalMessageSource<T> extends WriteableMessageSource<T>, ReadableMessageSource<T> {}
