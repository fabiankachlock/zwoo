import { Logger } from '../logging/logImport';
import { Backend, Endpoint } from './apiConfig';
import { WithBackendError, parseBackendError, BackendErrorAble, BackendErrorType } from './errors';

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
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
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
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
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
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
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
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
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
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
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

  static performChangePassword = async (oldPassword: string, newPassword: string): Promise<BackendErrorType | undefined> => {
    Logger.Api.log('performing change password action');
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking change password response');
      return undefined;
    }

    const response = await fetch('TODO', {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        password: oldPassword, // TODO
        newPassword: newPassword
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while changing password');
      return parseBackendError(await response.text());
    }

    return undefined;
  };

  static verifyAccount = async (id: string, code: string): Promise<BackendErrorAble<boolean>> => {
    Logger.Api.log(`verifying account ${id} with code ${code}`);
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
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
}
