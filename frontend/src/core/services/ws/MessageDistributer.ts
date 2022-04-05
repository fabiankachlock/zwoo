import { AsyncMessageQueue } from '../helper/MessageQueue';
import Logger from '../logging/logImport';
import { ZRPCoder } from '../zrp/zrpCoding';
import { BidirectionalMessageSource } from '../zrp/zrpInterfaces';
import { ZRPMessage } from '../zrp/zrpTypes';
import { GameWebsocket } from './Websocket';

export class ZRPWebsocketAdapter implements BidirectionalMessageSource<ZRPMessage> {
  private ws: GameWebsocket;
  private messageQueue: AsyncMessageQueue;

  constructor(public readonly url: string, public readonly gameId: string) {
    this.ws = new GameWebsocket(url);
    this.messageQueue = new AsyncMessageQueue();
    if (this.messageQueue.isStopped) {
      this.messageQueue.continue();
    }
    Logger.Websocket.log('[distributer] constructed');
  }

  public readMessages(handler: (message: ZRPMessage) => void): void {
    Logger.Websocket.log('[distributer] reading messages');
    this.ws.onMessage(raw => {
      handler(ZRPCoder.decode(raw));
    });
  }

  public writeMessage(message: ZRPMessage): void {
    Logger.Websocket.log('[distributer] scheduling send');
    this.messageQueue.execute(async () => {
      Logger.Websocket.log('[distributer] sending');
      this.ws.sendMessage(ZRPCoder.encode(message));
    });
  }

  public close() {
    Logger.Websocket.log('[distributer] closing queue');
    this.messageQueue.stop();
    this.ws.close();
  }
}
