import { AppConfig } from '@/config';
import { Logger } from '@/core/services/logging/logImport';

import { Backend, Endpoint } from './ApiConfig';
import { FetchResponse, WrappedFetch } from './FetchWrapper';

export class AccountService {
  static performChangePassword = async (oldPassword: string, newPassword: string): Promise<FetchResponse<undefined>> => {
    Logger.Api.log('performing change password action');

    const response = await WrappedFetch<undefined>(Backend.getUrl(Endpoint.ChangePassword), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        oldPassword: oldPassword,
        newPassword: newPassword
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while changing password');
    }

    return response;
  };

  static requestPasswordReset = async (email: string, lng: string | null = null): Promise<FetchResponse<undefined>> => {
    Logger.Api.log('performing request password reset action');

    const response = await WrappedFetch<undefined>(Backend.getUrlWithQuery(Endpoint.RequestPasswordReset, { lng: lng }), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        email: email
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while requesting password reset');
    }

    return response;
  };

  static performResetPassword = async (code: string, password: string): Promise<FetchResponse<undefined>> => {
    Logger.Api.log('performing reset password action');

    const response = await WrappedFetch<undefined>(Backend.getUrl(Endpoint.ResetPassword), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        code: code,
        password: password
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while resetting password');
    }

    return response;
  };

  static loadSettings = async (): Promise<string | undefined> => {
    Logger.Api.log('loading user settings');

    const response = await WrappedFetch<{ settings: string }>(Backend.getUrl(Endpoint.UserSettings), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      }
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while loading settings');
      return undefined;
    }
    return response.data?.settings;
  };

  static storeSettings = async (settings: string): Promise<FetchResponse<undefined>> => {
    Logger.Api.log('storing user settings');

    const response = await WrappedFetch<undefined>(Backend.getUrl(Endpoint.UserSettings), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      },
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({ settings: settings })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while loading settings');
    }

    return response;
  };
}
