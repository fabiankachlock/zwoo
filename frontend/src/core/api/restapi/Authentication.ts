import { AppConfig } from '@/config';
import { Logger } from '@/core/services/logging/logImport';

import { FetchResponse } from '../ApiEntities';
import { Login, UserSession } from '../entities/Authentication';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class AuthenticationService {
  private readonly api: Backend;

  public constructor(api: Backend) {
    this.api = api;
  }

  getUserInfo = async (): FetchResponse<UserSession> => {
    Logger.Api.log('fetching user auth status');

    const response = await WrappedFetch<UserSession>(this.api.getUrl(Endpoint.UserInfo), {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while fetching user auth status');
      return response;
    }

    return response;
  };

  performLogin = async (data: Login): FetchResponse<UserSession> => {
    Logger.Api.log(`logging in as ${data.email}`);

    const response = await WrappedFetch(this.api.getUrl(Endpoint.AccountLogin), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        email: data.email,
        password: data.password,
        captchaToken: data.captchaToken
      })
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while logging in');
      return response;
    }

    return await this.getUserInfo();
  };

  performLogout = async (): FetchResponse<undefined> => {
    Logger.Api.log('performing logout action');

    const response = await WrappedFetch(this.api.getUrl(Endpoint.AccountLogout), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      }
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while logging out');
      return response;
    }

    return response;
  };
}
