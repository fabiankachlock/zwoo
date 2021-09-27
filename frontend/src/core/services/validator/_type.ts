import i18n from '@/i18n';

export class ValidationResult {
  constructor(public isValid: boolean, private error: string | string[]) {}

  private transformError = (key: string) => `${i18n.global.t('errors.error')}: ${i18n.global.t(key)}`;

  public getErrors = (): string[] => {
    if (this.isValid) return [];

    if (typeof this.error === 'string') {
      return [this.transformError(this.error)];
    }

    return this.error.map(this.transformError);
  };
}

export abstract class Validator<Data> {
  abstract validate: (data: Data) => ValidationResult;
}
