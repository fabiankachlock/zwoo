import { Logger } from '../logging/logImport';
import { Backend, Endpoint } from './apiConfig';
import { WithBackendError, parseBackendError } from './errors';

type UserInfo = {
  username: string;
  email: string;
};

export type AuthenticationStatus =
  | {
      username: string;
      email: string;
      isLoggedIn: true;
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
      email: data.email
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
}
