import { WithBackendError } from '../ApiError';

export type NewUser = {
  username: string;
  email: string;
  password: string;
  beta?: string;
  captchaToken: string;
};

export type UserLogin = {
  login: string;
  password: string;
  captchaToken: string;
};

export type UserInfo = {
  username: string;
  email: string;
  wins: number;
};

export type AuthenticationStatus =
  | {
      username: string;
      email: string;
      isLoggedIn: true;
      wins?: number;
    }
  | WithBackendError<{
      isLoggedIn?: false;
    }>;
