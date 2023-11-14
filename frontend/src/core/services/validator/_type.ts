export class ValidationResult {
  constructor(
    public isValid: boolean,
    private error: string | string[]
  ) {}

  public getErrors = (): string[] => {
    if (this.isValid) return [];

    if (typeof this.error === 'string') {
      return [this.error];
    }

    return [...(typeof this.error === 'string' ? [this.error] : this.error)];
  };
}

export abstract class Validator<Data> {
  abstract validate: (data: Data) => ValidationResult;
}
