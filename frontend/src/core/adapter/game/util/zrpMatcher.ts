import { ZRPMessage, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

export interface ZRPMatcher<T = unknown> {
  matches(msg: ZRPMessage<T>): boolean;
}

export class ZRPCombinedMatcher<T> implements ZRPMatcher<T> {
  private matchers: ((msg: ZRPMessage<T>) => boolean)[];

  constructor(...matchers: ((msg: ZRPMessage<T>) => boolean)[]) {
    this.matchers = matchers;
  }

  public matches(msg: ZRPMessage<T>): boolean {
    for (const matcher of this.matchers) {
      if (matcher(msg)) {
        return true;
      }
    }
    return false;
  }

  public extendWith(matcher: (msg: ZRPMessage<T>) => boolean): void {
    this.matchers.push(matcher);
  }
}

export const createZRPOPCodeMatcher = (...codes: ZRPOPCode[]) => {
  return new ZRPCombinedMatcher<(typeof codes)[number]>(
    ...codes.map(code => {
      return (msg: ZRPMessage) => msg.code === code;
    })
  );
};

export const createZRPCustomMatcher = <T>(matcher: (msg: ZRPMessage<T>) => boolean) => {
  return new ZRPCombinedMatcher(matcher);
};
