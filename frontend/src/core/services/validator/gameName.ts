import { ValidationResult, Validator } from './_type';

export class GameNameValidator implements Validator<string> {
  public validate = (name: string): ValidationResult => {
    return new ValidationResult(name.length >= 3, 'errors.gameNameToShort');
  };
}
