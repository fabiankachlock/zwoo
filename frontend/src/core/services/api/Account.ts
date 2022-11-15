import { AppConfig } from '@/config';

import { Logger } from '../logging/logImport';
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

  static requestPasswordReset = async (email: string): Promise<FetchResponse<undefined>> => {
    Logger.Api.log('performing request password reset action');

    const response = await WrappedFetch<undefined>(Backend.getUrl(Endpoint.RequestPasswordReset), {
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
}
