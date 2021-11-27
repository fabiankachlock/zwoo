import { ValidationResult, Validator } from './_type';

export class PasswordMatchValidator implements Validator<[string, string]> {
  public validate = (passwords: [string, string]): ValidationResult => {
    if (passwords.length < 2) return new ValidationResult(false, 'Invalid Args');
    return new ValidationResult(passwords[0] === passwords[1], 'errors.passwordsDontMatch');
  };
}

