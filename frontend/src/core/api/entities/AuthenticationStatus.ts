import { WithBackendError } from '../ApiError';

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
