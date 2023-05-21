import { ZRPCoder } from '../zrpCoding';
import { ZRPOPCode } from '../zrpTypes';

describe('ZRPCoder', () => {
  describe('encode', () => {
    it('should encode correct', () => {
      expect(
        ZRPCoder.encode({
          code: 111,
          data: {}
        })
      ).toEqual('111,{}');
    });

    it('should always encode into 3 digits', () => {
      expect(
        ZRPCoder.encode({
          code: 1 as ZRPOPCode,
          data: {}
        })
      ).toEqual('001,{}');
    });
  });

  describe('decode', () => {
    it('should decode correct', () => {
      expect(ZRPCoder.decode('111,{}')).toEqual({
        code: 111,
        data: {}
      });
    });

    it('should catch json decoding errors', () => {
      expect(() => {
        expect(ZRPCoder.decode('111,{abc}').code).toEqual(ZRPOPCode._DecodingError);
      }).not.toThrow();
    });

    it('should catch invalid codes', () => {
      expect(() => {
        expect(ZRPCoder.decode('aab,{}').code).toEqual(ZRPOPCode._DecodingError);
        expect(ZRPCoder.decode(',{gst}').code).toEqual(ZRPOPCode._DecodingError);
      }).not.toThrow();
    });

    it('should catch invalid message format', () => {
      expect(() => {
        expect(ZRPCoder.decode('xxx').code).toEqual(ZRPOPCode._DecodingError);
        expect(ZRPCoder.decode(',').code).toEqual(ZRPOPCode._DecodingError);
      }).not.toThrow();
    });
  });
});
