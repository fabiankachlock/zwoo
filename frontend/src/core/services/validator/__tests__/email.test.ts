import { EmailValidator } from '@/core/services/validator/email';

describe('services > validator > email', () => {
  // credits: gist.github.com/cjaoude/fd9910626629b53c4d25

  const valid = [
    'email@example.com',
    'firstname.lastname@example.com',
    'email@subdomain.example.com',
    'firstname+lastname@example.com',
    'email@123.123.123.123',
    'email@[123.123.123.123]',
    '"email"@example.com',
    '1234567890@example.com',
    'email@example-one.com',
    '_______@example.com',
    'email@example.name',
    'email@example.museum',
    'email@example.co.jp',
    'firstname-lastname@example.com'
  ];

  const invalid = [
    'plainaddress',
    '#@%^%#$@#$@#.com',
    '@example.com',
    'Joe Smith <email@example.com>',
    'email.example.com',
    'email@example@example.com',
    '.email@example.com',
    'email.@example.com',
    'email..email@example.com',
    'あいうえお@example.com',
    'email@example.com (Joe Smith)',
    'email@example',
    'email@-example.com',
    'email@example..com',
    'Abc..123@example.com',
    '”(),:;<>[]@example.com',
    'just”not”right@example.com',
    'this is"really"notallowed@example.com'
  ];

  const validator = new EmailValidator();

  it('should detect all valid addresses', () => {
    valid.forEach(email => {
      const isValid = validator.validate(email).isValid;
      if (!isValid) console.log(email);
      expect(isValid).toBe(true);
    });
  });

  it('should detect all invalid addresses', () => {
    invalid.forEach(email => {
      const isValid = validator.validate(email).isValid;
      if (isValid) console.log(email);
      expect(isValid).toBe(false);
    });
  });
});

