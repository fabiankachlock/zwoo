import { CaptchaResponse } from '@/core/api/entities/Captcha';

import { ValidationResult, Validator } from './_type';

export const MIN_RECAPTCHA_SCORE = 0.5;

export class RecaptchaValidator implements Validator<CaptchaResponse | undefined> {
  public validate = (response: CaptchaResponse | undefined): ValidationResult => {
    return new ValidationResult(response ? response.success && response.score > MIN_RECAPTCHA_SCORE : false, 'errors.recaptcha');
  };
}
