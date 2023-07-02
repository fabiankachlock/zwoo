import { ValidationResult, Validator } from './_type';

export class CaptchaValidator implements Validator<string | undefined> {
  public validate = (token: string | undefined): ValidationResult => {
    return new ValidationResult(!!token, 'errors.captcha');
  };
}
