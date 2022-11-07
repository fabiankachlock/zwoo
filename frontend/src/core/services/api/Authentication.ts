import { AppConfig } from '@/config';

import { Logger } from '../logging/logImport';
import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble, parseBackendError, WithBackendError } from './Errors';

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
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking auth status response');
      return {
        isLoggedIn: false
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.UserInfo), {
      method: 'GET',
      credentials: 'include'
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while fetching user auth status');
      return {
        isLoggedIn: false,
        error: parseBackendError(await response.text())
      };
    }

    const data = (await response.json()) as UserInfo;
    return {
      isLoggedIn: true,
      username: data.username,
      email: data.email,
      wins: data.wins
    };
  };

  static performLogin = async (email: string, password: string): Promise<AuthenticationStatus> => {
    Logger.Api.log(`logging in as ${email}`);
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking login response');
      return {
        username: 'test-user',
        email: 'test@test.com',
        isLoggedIn: true
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.AccountLogin), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: email,
        password: password
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while logging in');
      return {
        isLoggedIn: false,
        error: parseBackendError(await response.text())
      };
    }

    return await AuthenticationService.getUserInfo();
  };

  static performLogout = async (): Promise<AuthenticationStatus> => {
    Logger.Api.log('performing logout action');
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking logout response');
      return {
        isLoggedIn: false
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.AccountLogout), {
      method: 'GET',
      credentials: 'include'
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while logging out');
      return {
        isLoggedIn: false,
        error: parseBackendError(await response.text())
      };
    }

    return {
      isLoggedIn: false
    };
  };

  static performCreateAccount = async (username: string, email: string, password: string, beta?: string): Promise<AuthenticationStatus> => {
    Logger.Api.log(`performing create account action of ${username} with ${email}`);
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking create account response');
      return {
        username: 'test-user',
        email: 'test@test.com',
        isLoggedIn: true
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.CreateAccount), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username: username,
        email: email,
        password: password,
        code: beta
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while creating an account');
      return {
        isLoggedIn: false,
        error: parseBackendError(await response.text())
      };
    }

    return {
      isLoggedIn: false // users can only log in, when the account is verified
    };
  };

  static performDeleteAccount = async (password: string): Promise<AuthenticationStatus> => {
    Logger.Api.log('performing delete account action');
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking delete account response');
      return {
        isLoggedIn: false
      };
    }

    const response = await fetch(Backend.getUrl(Endpoint.DeleteAccount), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        password: password
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while deleting account');
      return {
        isLoggedIn: false,
        error: parseBackendError(await response.text())
      };
    }

    return {
      isLoggedIn: false
    };
  };

  static verifyAccount = async (id: string, code: string): Promise<BackendErrorAble<boolean>> => {
    Logger.Api.log(`verifying account ${id} with code ${code}`);
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking verify response');
      await new Promise(r => {
        setTimeout(() => r({}), 5000);
      });
      return Math.random() > 0.5;
    }

    const response = await fetch(
      Backend.getDynamicUrl(Endpoint.AccountVerify, {
        code,
        id
      }),
      {
        method: 'GET'
      }
    );

    if (response.status !== 200) {
      Logger.Api.warn('cant verify account');
      return {
        error: parseBackendError(await response.text())
      };
    }

    return true;
  };

  static resendVerificationEmail = async (email: string): Promise<BackendErrorAble<boolean>> => {
    Logger.Api.log(`resending verification email of ${email}`);
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking resend verify email response');
      return true;
    }

    const response = await fetch(Backend.getUrl(Endpoint.ResendVerificationEmail), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: email
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('cant resend verification email');
      return {
        error: parseBackendError(await response.text())
      };
    }

    return true;
  };
}
