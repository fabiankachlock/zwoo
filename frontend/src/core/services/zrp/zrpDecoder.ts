import { ZRPMessage, ZRPOPCode } from './zrpTypes';

export class ZRPDecoder {
  static decode(msg: string): ZRPMessage {
    try {
      const content = JSON.parse(msg) as ZRPMessage;
      return content;
    } catch (e: unknown) {
      return {
        code: ZRPOPCode._DecodingError,
        payload: {
          thrownError: e,
          timestamp: performance.now(),
          rawReceivedContent: msg
        }
      };
    }
  }
}
