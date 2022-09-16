export type BackendErrorType = {
  message: string;
  code: BackendError;
};

export type BackendErrorAble<T> =
  | {
      error?: BackendErrorType;
    }
  | T;

export type WithBackendError<T> = {
  error?: BackendErrorType;
} & T;

export enum BackendError {
  _UnknownError = -1,
  BackendError = 100,

  MissingCookie = 111,
  UserNotFound = 112,
  PasswordDoNotMatch = 113,
  SessionIdNotMatching = 114,
  InvalidEmail = 115,
  InvalidUsername = 116,
  InvalidPassword = 117,

  EmailAlreadyTaken = 120,
  UsernameAlreadyTaken = 121,
  AccountFailedToVerify = 122,

  DeletingUserFailed = 135,

  GameNotFound = 140,
  GameNameMissing = 141,
  JoinFailed = 142,
  OpcodeMissing = 143,
  InvalidOpcode = 144,
  InvalidGameId = 145,
  AlreadyPlaying = 146
}

export const parseBackendError = (text: string): BackendErrorType => {
  try {
    const content = JSON.parse(text);
    if ('code' in content) {
      return content as BackendErrorType;
    }
    throw content;
  } catch (e: unknown) {
    return {
      code: BackendError._UnknownError,
      message: JSON.stringify(e)
    };
  }
};

export const createEmptyBackendError = (): BackendErrorType => ({
  code: BackendError._UnknownError,
  message: ''
});

export const unwrapBackendError = <T>(value: BackendErrorAble<T> | WithBackendError<T>): [T, undefined] | [undefined, BackendErrorType] => {
  if (typeof value === 'object' && value && 'error' in value) {
    return [undefined, value.error as BackendErrorType];
  }
  return [value as T, undefined];
};

export const getBackendErrorTranslation = (err: BackendErrorType): string => {
  return `errors.backend.${err.code}`;
};
