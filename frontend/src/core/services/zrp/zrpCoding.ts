import { ZRPMessage, ZRPOPCode } from './zrpTypes';

export class ZRPCoder {
  static decode(msg: string): ZRPMessage {
    try {
      const content = JSON.parse(msg) as ZRPMessage;
      return content;
    } catch (e: unknown) {
      return {
        code: ZRPOPCode._DecodingError,
        data: {
          thrownError: e,
          timestamp: performance.now(),
          rawReceivedContent: msg
        }
      };
    }
  }

  static encode(msg: ZRPMessage): string {
    return JSON.stringify(msg);
  }
}
