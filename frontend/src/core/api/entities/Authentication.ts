export type UserSession = {
  username: string;
  email: string;
  wins: number;
};

export type CreateAccount = {
  username: string;
  email: string;
  password: string;
  acceptedTerms: boolean;
  code: string;
  captchaToken: string;
};

export type Login = {
  email: string;
  password: string;
  captchaToken: string;
};

export type DeleteAccount = {
  password: string;
};

export type VerifyEmail = {
  email: string;
};

export type UserSettings = {
  settings: string;
};

export type ChangePassword = {
  oldPassword: string;
  newPassword: string;
};

export type RequestPasswordReset = {
  email: string;
  captchaToken: string;
};

export type ResetPassword = {
  code: string;
  password: string;
  captchaToken: string;
};
