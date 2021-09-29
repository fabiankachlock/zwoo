import { ValidationResult, Validator } from './_type';

export class UsernameValidator implements Validator<string> {
  public validate = (username: string) => {
    return new ValidationResult(username.length >= 4, 'errors.usernameToShort');
  };
}
