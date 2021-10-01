import { PasswordMatchValidator } from '@/core/services/validator/passwordMatch';

describe('services > validator > passwordMatch', () => {
  const validator = new PasswordMatchValidator();

  const testCases = [
    {
      a: '123',
      b: '345',
      expected: false
    },
    {
      a: 'abc',
      b: 'abC',
      expected: false
    },
    {
      a: 'abc123',
      b: 'abc123',
      expected: true
    }
  ];

  it('should detect non matching passwords', () => {
    testCases.forEach(testCase => {
      expect(validator.validate([testCase.a, testCase.b]).isValid).toBe(testCase.expected);
    });
  });
});
