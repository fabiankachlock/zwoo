import { ValidationResult, Validator } from './_type';

const NumberRegex = /[0-9]+/;
const SpecialCharacterRegex = /[!#$%&'*+/=?^_´{|}\-[\]]+/;

export class PasswordValidator implements Validator<string> {
  public validate = (password: string) => {
    const containsNumber = NumberRegex.test(password);
    const containsSpecialChar = SpecialCharacterRegex.test(password);
    const longEnough = password.length > 8;
    const errors = [] as string[];

    if (!longEnough) {
      errors.push('error.passwordNotLongEnough');
    }

    if (!containsNumber) {
      errors.push('errors.shouldContainNumber');
    }

    if (!containsSpecialChar) {
      errors.push('errors.shouldContainSpecialChar');
    }

    return new ValidationResult(longEnough && containsNumber && containsSpecialChar, errors);
  };
}
