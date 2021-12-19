import { ValidationResult, Validator } from './_type';

export class UsernameValidator implements Validator<string> {
  public validate = (username: string): ValidationResult => {
    if (username.length > 20) {
      return new ValidationResult(false, 'errors.inputTooLong');
    }
    return new ValidationResult(username.length >= 4, 'errors.usernameToShort'); // 20
  };
}
