import { ZRPCoder } from '../zrp/zrpCoding';
import { ZRPMessage, ZRPOPCode } from '../zrp/zrpTypes';

export class GameWebsocket {
  private connection: WebSocket;

  constructor(public readonly gameId: string) {
    this.connection = new WebSocket('...'); // TODO: insert ws url
    this.connection.onmessage = this.handleMessage;
    this.connection.onerror = this.handleError;
    this.connection.onopen = this.handleOpen;
    this.connection.onclose = this.handleClose;
  }

  private handleMessage = (evt: MessageEvent) => {
    this.messageHandler(evt.data);
  };

  private handleOpen = () => {
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._Connected,
        payload: {}
      } as ZRPMessage<ZRPOPCode._Connected>)
    );
  };

  private handleError = (evt: Event) => {
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._ConnectionError,
        payload: {
          timestamp: performance.now(),
          thrownError: evt,
          rawReceivedContent: ''
        }
      } as ZRPMessage<ZRPOPCode._ConnectionError>)
    );
  };

  private handleClose = (evt: CloseEvent) => {
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._ConnectionClosed,
        payload: {
          timestamp: performance.now(),
          thrownError: evt,
          rawReceivedContent: ''
        }
      } as ZRPMessage<ZRPOPCode._ConnectionClosed>)
    );
  };

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  private messageHandler: (nsg: string) => void = () => {};
  public onMessage(handler: (msg: string) => void): void {
    this.messageHandler = handler;
  }

  public sendMessage(msg: string) {
    this.connection.send(msg);
  }

  public close() {
    this.connection.close(1000 /* normal closure */);
  }
}
