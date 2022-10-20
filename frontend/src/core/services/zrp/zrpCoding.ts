import { ZRPMessage, ZRPOPCode } from './zrpTypes';

export class ZRPCoder {
  private static DecodeRegex = /(?<code>[0-9]{3}),(?<data>.*)/;

  static decode(msg: string): ZRPMessage {
    try {
      const result = this.DecodeRegex.exec(msg);
      if (!result?.groups) throw new Error('no regex match');
      const { code, data } = result.groups;
      const numericCode = parseInt(code, 10);
      if (isNaN(numericCode)) throw new Error('invalid opcode');
      return {
        code: numericCode,
        data: JSON.parse(data)
      };
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
    return `${msg.code.toString().padStart(3, '0')},${JSON.stringify(msg.data)}`;
  }

  static isInternalMessage(code: ZRPOPCode): boolean {
    return code >= 900 && code < 1000;
  }
}
