import { AppConfig } from '@/config';
import { Logger } from '@/core/services/logging/logImport';

import { AuthenticationStatus, UserInfo, UserLogin } from '../entities/Authentication';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

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

    if (response.error || !response.data) {
      Logger.Api.warn('received erroneous response while fetching user auth status');
      return {
        isLoggedIn: false,
        error: response.error
      };
    }

    const data = response.data;
    return {
      isLoggedIn: true,
      username: data.username,
      email: data.email,
      wins: data.wins
    };
  };

  static performLogin = async (data: UserLogin): Promise<AuthenticationStatus> => {
    Logger.Api.log(`logging in as ${data.login}`);

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
        email: data.login,
        password: data.password,
        captchaToken: data.captchaToken
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
}
