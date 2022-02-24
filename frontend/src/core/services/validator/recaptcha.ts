import { ReCaptchaResponse } from '../api/reCAPTCHA';
import { ValidationResult, Validator } from './_type';

export const MIN_RECAPTCHA_SCORE = 0.5;

export class RecaptchaValidator implements Validator<ReCaptchaResponse | undefined> {
  public validate = (response: ReCaptchaResponse | undefined): ValidationResult => {
    return new ValidationResult(response ? response.success && response.score > MIN_RECAPTCHA_SCORE : false, 'errors.recaptcha');
  };
}
