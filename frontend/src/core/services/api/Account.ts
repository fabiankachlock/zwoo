import { Logger } from '../logging/logImport';
import { Backend, Endpoint } from './apiConfig';
import { parseBackendError, BackendErrorType } from './errors';

export class AccountService {
  static performChangePassword = async (oldPassword: string, newPassword: string): Promise<BackendErrorType | undefined> => {
    Logger.Api.log('performing change password action');
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking change password response');
      return undefined;
    }

    const response = await fetch(Backend.getUrl(Endpoint.ChangePassword), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        oldPassword: oldPassword,
        newPassword: newPassword
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while changing password');
      return parseBackendError(await response.text());
    }

    return undefined;
  };

  static requestPasswordReset = async (email: string): Promise<BackendErrorType | undefined> => {
    Logger.Api.log('performing request password reset action');
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking request password reset response');
      return undefined;
    }

    const response = await fetch(Backend.getUrl(Endpoint.RequestPasswordReset), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: email
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while requesting password reset');
      return parseBackendError(await response.text());
    }

    return undefined;
  };

  static performResetPassword = async (code: string, password: string): Promise<BackendErrorType | undefined> => {
    Logger.Api.log('performing reset password action');
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking reset password response');
      return undefined;
    }

    const response = await fetch(Backend.getUrl(Endpoint.ResetPassword), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        code: code,
        password: password
      })
    });

    if (response.status !== 200) {
      Logger.Api.warn('received erroneous response while resetting password');
      return parseBackendError(await response.text());
    }

    return undefined;
  };
}
