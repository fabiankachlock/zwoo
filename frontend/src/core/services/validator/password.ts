import { ValidationResult, Validator } from './_type';

const NumberRegex = /[0-9]+/;
const SpecialCharacterRegex = /[!#$%&'*+/=?^_´{|}\-[\]]+/;
const NormalCharacterRegex = /[a-zA-Z]+/;

export class PasswordValidator implements Validator<string> {
  public validate = (password: string): ValidationResult => {
    const containsNumber = NumberRegex.test(password);
    const containsNormalChar = NormalCharacterRegex.test(password);
    const containsSpecialChar = SpecialCharacterRegex.test(password);
    const longEnough = password.length >= 8;
    const errors = [] as string[];

    if (!longEnough) {
      errors.push('errors.passwordNotLongEnough');
    }

    if (!containsNumber) {
      errors.push('errors.shouldContainNumber');
    }

    if (!containsNormalChar) {
      errors.push('errors.shouldContainNormalChar');
    }

    if (!containsSpecialChar) {
      errors.push('errors.shouldContainSpecialChar');
    }

    return new ValidationResult(longEnough && containsNumber && containsSpecialChar, errors);
  };
}
