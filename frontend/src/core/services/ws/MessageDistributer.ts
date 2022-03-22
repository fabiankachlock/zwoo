import { AsyncMessageQueue } from '../helper/MessageQueue';
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
  }

  public readMessages(handler: (message: ZRPMessage) => void): void {
    this.ws.onMessage(raw => {
      handler(ZRPCoder.decode(raw));
    });
  }

  public writeMessage(message: ZRPMessage): void {
    this.messageQueue.execute(async () => {
      this.ws.sendMessage(ZRPCoder.encode(message));
    });
  }

  public close() {
    this.messageQueue.stop();
    this.ws.close();
  }
}
