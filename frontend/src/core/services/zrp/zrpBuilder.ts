import { ZRPMessage, ZRPOPCode, ZRPPayloadMap } from './zrpTypes';

export class ZRPMessageBuilder {
  static build<C extends ZRPOPCode>(code: C, payload: ZRPPayloadMap[C]): ZRPMessage<C> {
    return {
      code: code,
      data: payload
    } as ZRPMessage<C>;
  }
}
