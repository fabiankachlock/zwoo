import { ZRPOPCode, ZRPMessage } from '@/core/services/zrp/zrpTypes';
import { WatchStopHandle } from 'vue';
import { useWatchGameEvents } from './gameEventWatcher';
import { createZRPOPCodeMatcher, ZRPMatcher } from './zrpMatcher';

export class MonolithicEventWatcher<EventType extends ZRPOPCode[]> {
  private _matcher: ZRPMatcher<EventType[number]>;
  private events: EventType;

  private stopWatcher: WatchStopHandle | undefined;

  constructor(...events: EventType) {
    this.events = events;
    this._matcher = createZRPOPCodeMatcher(ZRPOPCode._Connected, ZRPOPCode._ConnectionClosed, ...events);
    this.stopWatcher = undefined;
    this.restart();
  }

  public stop() {
    if (this.stopWatcher) {
      this.stopWatcher();
      this.stopWatcher = undefined;
    }
  }

  public restart() {
    if (this.stopWatcher) {
      this.stop();
    }
    this.stopWatcher = useWatchGameEvents(this._matcher, this.eventHandler);
  }

  private eventHandler = (msg: ZRPMessage<EventType[number] | ZRPOPCode._ConnectionClosed | ZRPOPCode._Connected>) => {
    if (msg.code === ZRPOPCode._Connected) {
      this._openHandler();
    } else if (msg.code === ZRPOPCode._ConnectionClosed) {
      this._closeHandler();
    } else {
      this._msgHandler(msg);
    }
  };

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  private _msgHandler: (msg: ZRPMessage<EventType[number]>) => void = () => {};
  public onMessage(handler: (msg: ZRPMessage<EventType[number]>) => void) {
    this._msgHandler = handler;
  }

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  private _openHandler: () => void = () => {};
  public onOpen(handler: () => void) {
    this._openHandler = handler;
  }

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  private _closeHandler: () => void = () => {};
  public onClose(handler: () => void) {
    this._closeHandler = handler;
  }
}
