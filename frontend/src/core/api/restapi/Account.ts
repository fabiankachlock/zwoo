// eslint-disable-next-line simple-import-sort/imports
import { AppConfig } from '@/config';
import { Logger } from '@/core/services/logging/logImport';
import { FetchResponse } from '../ApiEntities';

import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';
import { BackendErrorAble } from '../ApiError';
import { AuthenticationStatus, NewUser } from '../entities/Authentication';

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

  static requestPasswordReset = async (email: string, captchaToken: string, lng: string | null = null): Promise<FetchResponse<undefined>> => {
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
        email: email,
        captchaToken: captchaToken
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while requesting password reset');
    }

    return response;
  };

  static performResetPassword = async (code: string, password: string, captchaToken: string): Promise<FetchResponse<undefined>> => {
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
        password: password,
        captchaToken: captchaToken
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

  static performCreateAccount = async (data: NewUser, lng: string | null = null): Promise<AuthenticationStatus> => {
    Logger.Api.log(`performing create account action of ${data.username} with ${data.email}`);

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
        username: data.username,
        email: data.email,
        password: data.password,
        code: data.beta,
        acceptedTerms: data.acceptedTerms,
        captchaToken: data.captchaToken
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
