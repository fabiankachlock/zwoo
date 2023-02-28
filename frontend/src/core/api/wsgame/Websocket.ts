import { ZRPCoder } from '@/core/domain/zrp/zrpCoding';
import { RealtimeGameMessageAdapter } from '@/core/domain/zrp/zrpInterfaces';
import { ZRPMessage, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';
import Logger from '@/core/services/logging/logImport';

export class GameWebsocket implements RealtimeGameMessageAdapter {
  private connection: WebSocket;

  get state(): number {
    return this.connection.readyState;
  }

  constructor(public readonly url: string) {
    this.connection = new WebSocket(url);
    this.connection.onmessage = this.handleMessage;
    this.connection.onerror = this.handleError;
    this.connection.onopen = this.handleOpen;
    this.connection.onclose = this.handleClose;
  }

  private handleMessage = (evt: MessageEvent) => {
    Logger.Websocket.debug(`received: ${evt.data}`);
    this.messageHandler(evt.data);
  };

  private handleOpen = () => {
    Logger.Websocket.log('connection opened');
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._Connected,
        data: {}
      } as ZRPMessage<ZRPOPCode._Connected>)
    );
  };

  private handleError = (evt: Event) => {
    Logger.Websocket.warn(`error event: ${JSON.stringify(evt)}`);
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._ConnectionError,
        data: {
          timestamp: performance.now(),
          thrownError: evt,
          rawReceivedContent: ''
        }
      } as ZRPMessage<ZRPOPCode._ConnectionError>)
    );
  };

  private handleClose = (evt: CloseEvent) => {
    Logger.Websocket.debug(`close event: ${JSON.stringify(evt)}`);
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._ConnectionClosed,
        data: {
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
    Logger.Websocket.log(`sending: ${msg}`);
    this.connection.send(msg);
  }

  public close() {
    Logger.Websocket.log('closing websocket from client');
    this.connection.close(1000 /* normal closure */);
  }
}
