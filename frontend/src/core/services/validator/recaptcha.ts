import { ReCaptchaResponse } from '../api/reCAPTCHA';
import { ValidationResult, Validator } from './_type';

export const MIN_RECAPTCHA_SCORE = 0.3;

export class RecaptchaValidator implements Validator<ReCaptchaResponse> {
  public validate = (response: ReCaptchaResponse): ValidationResult => {
    return new ValidationResult(response.success && response.score > MIN_RECAPTCHA_SCORE, 'errors.recaptcha');
  };
}
