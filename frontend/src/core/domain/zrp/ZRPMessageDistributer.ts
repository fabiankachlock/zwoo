import { ZRPCoder } from '@/core/domain/zrp/zrpCoding';
import { BidirectionalMessageSource, RealtimeGameMessageAdapter } from '@/core/domain/zrp/zrpInterfaces';
import { ZRPMessage } from '@/core/domain/zrp/zrpTypes';
import { AsyncMessageQueue } from '@/core/helper/MessageQueue';
import Logger from '@/core/services/logging/logImport';

export class ZRPWebsocketAdapter implements BidirectionalMessageSource<ZRPMessage> {
  private messageQueue: AsyncMessageQueue;

  constructor(private readonly ws: RealtimeGameMessageAdapter) {
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
      while (this.ws.state === WebSocket.CONNECTING) {
        Logger.Websocket.warn(`connection istn't opened yet, retrying in 100ms`);
        await new Promise(res => setTimeout(() => res({}), 100));
      }
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
