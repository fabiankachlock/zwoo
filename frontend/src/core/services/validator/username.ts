import { ValidationResult, Validator } from './_type';

export class UsernameValidator implements Validator<string> {
  public validate = (username: string): ValidationResult => {
    return new ValidationResult(username.length >= 4, 'errors.usernameToShort');
  };
}

