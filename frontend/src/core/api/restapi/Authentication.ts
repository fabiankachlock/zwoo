import { AppConfig } from '@/config';
import { Logger } from '@/core/services/logging/logImport';

import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble, WithBackendError } from './Errors';
import { WrappedFetch } from './FetchWrapper';

type UserInfo = {
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

export class AuthenticationService {
  static getUserInfo = async (): Promise<AuthenticationStatus> => {
    Logger.Api.log('fetching user auth status');

    const response = await WrappedFetch<UserInfo>(Backend.getUrl(Endpoint.UserInfo), {
      useBackend: AppConfig.UseBackend,
      method: 'GET',
      fallbackValue: {
        email: 'test@example.com',
        username: 'test-user',
        wins: 5
      },
      requestOptions: {
        withCredentials: true
      }
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while fetching user auth status');
      return {
        isLoggedIn: false,
        error: response.error
      };
    }

    const data = response.data!;
    return {
      isLoggedIn: true,
      username: data.username,
      email: data.email,
      wins: data.wins
    };
  };

  static performLogin = async (email: string, password: string): Promise<AuthenticationStatus> => {
    Logger.Api.log(`logging in as ${email}`);

    const response = await WrappedFetch(Backend.getUrl(Endpoint.AccountLogin), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        email: email,
        password: password
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while logging in');
      return {
        isLoggedIn: false,
        error: response.error
      };
    }

    return await AuthenticationService.getUserInfo();
  };

  static performLogout = async (): Promise<AuthenticationStatus> => {
    Logger.Api.log('performing logout action');

    const response = await WrappedFetch(Backend.getUrl(Endpoint.AccountLogout), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      }
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while logging out');
      return {
        isLoggedIn: false,
        error: response.error
      };
    }

    return {
      isLoggedIn: false
    };
  };

  static performCreateAccount = async (
    username: string,
    email: string,
    password: string,
    beta?: string,
    lng: string | null = null
  ): Promise<AuthenticationStatus> => {
    Logger.Api.log(`performing create account action of ${username} with ${email}`);

    const response = await WrappedFetch(Backend.getUrlWithQuery(Endpoint.CreateAccount, { lng: lng }), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        username: username,
        email: email,
        password: password,
        code: beta
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while creating an account');
      return {
        isLoggedIn: false,
        error: response.error
      };
    }

    return {
      isLoggedIn: false // users can only log in, when the account is verified
    };
  };

  static performDeleteAccount = async (password: string): Promise<AuthenticationStatus> => {
    Logger.Api.log('performing delete account action');

    const response = await WrappedFetch(Backend.getUrl(Endpoint.DeleteAccount), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        password: password
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while deleting account');
      return {
        isLoggedIn: false,
        error: response.error
      };
    }

    return {
      isLoggedIn: false
    };
  };

  static verifyAccount = async (id: string, code: string): Promise<BackendErrorAble<boolean>> => {
    Logger.Api.log(`verifying account ${id} with code ${code}`);

    const response = await WrappedFetch(
      Backend.getDynamicUrl(Endpoint.AccountVerify, {
        code,
        id
      }),
      {
        useBackend: AppConfig.UseBackend,
        method: 'GET',
        responseOptions: {
          decodeJson: false
        }
      }
    );

    if (response.error) {
      Logger.Api.warn('cant verify account');
      return {
        error: response.error
      };
    }

    return true;
  };

  static resendVerificationEmail = async (email: string, lng: string | null = null): Promise<BackendErrorAble<boolean>> => {
    Logger.Api.log(`resending verification email of ${email}`);

    const response = await WrappedFetch(Backend.getUrlWithQuery(Endpoint.ResendVerificationEmail, { lng: lng }), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        email: email
      })
    });

    if (response.error) {
      Logger.Api.warn('cant resend verification email');
      return {
        error: response.error
      };
    }

    return true;
  };
}
