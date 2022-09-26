import { Logger } from '../logging/logImport';
import { Backend, Endpoint } from './apiConfig';
import { WithBackendError, parseBackendError, BackendErrorType } from './errors';

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
}
