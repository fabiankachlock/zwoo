import { ZRPCoder } from '@/core/domain/zrp/zrpCoding';
import { RealtimeGameMessageAdapter } from '@/core/domain/zrp/zrpInterfaces';
import { ZRPMessage, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';
import Logger from '@/core/services/logging/logImport';
import { CSharpExport } from '@/core/services/wasm/Interpop';
import { WasmManger } from '@/core/services/wasm/WasmManager';

export class WasmSocket implements RealtimeGameMessageAdapter {
  private _state: number;
  private _instance?: CSharpExport;

  get state(): number {
    return this._state;
  }

  constructor() {
    this._state = WebSocket.CONNECTING;
    this.setupHandler();
  }

  private async setupHandler() {
    this._instance = await WasmManger.global.getInstance();
    this._instance.LocalNotificationAdapter.OnMessage(this.handleMessage);
    this._instance.LocalNotificationAdapter.OnDisconnect(this.handleClose);
    this.handleOpen();
  }

  private handleMessage = (code: number, payload: string) => {
    console.log(code, payload, JSON.stringify(payload));
    Logger.Websocket.debug(`[wasm] received: ${code},${payload}`);
    // TODO: remove unnesessary encoding here
    this.messageHandler(ZRPCoder.encode({ code, data: JSON.parse(payload) }));
  };

  private handleOpen = () => {
    Logger.Websocket.log('[wasm] connection opened');
    this._state = WebSocket.OPEN;
    this.messageHandler(
      ZRPCoder.encode({
        code: ZRPOPCode._Connected,
        data: {}
      } as ZRPMessage<ZRPOPCode._Connected>)
    );
  };

  private handleClose = () => {
    Logger.Websocket.debug(`[wasm] connection disconnect`);
    this._state = WebSocket.CLOSED;
  };

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  private messageHandler: (msg: string) => void = () => {};
  public onMessage(handler: (msg: string) => void): void {
    this.messageHandler = handler;
  }

  public sendMessage(msg: string) {
    Logger.Websocket.log(`[wasm] sending: ${msg}`);
    const data = ZRPCoder.decode(msg);
    this._instance?.GameManager.SendEvent(data.code, data.data);
  }

  public close() {
    Logger.Websocket.log('[wasm] closing websocket from client');
    this._instance?.GameManager.CloseGame();
    this.handleClose();
  }
}
