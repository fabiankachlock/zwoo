// eslint-disable-next-line simple-import-sort/imports
import { AppConfig } from '@/config';
import { Logger } from '@/core/services/logging/logImport';
import { FetchResponse } from '../ApiEntities';

import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';
import { CreateAccount, UserSettings } from '../entities/Authentication';

export class AccountService {
  private readonly api: Backend;

  public constructor(api: Backend) {
    this.api = api;
  }

  performChangePassword = async (oldPassword: string, newPassword: string): FetchResponse<undefined> => {
    Logger.Api.log('performing change password action');

    const response = await WrappedFetch<undefined>(this.api.getUrl(Endpoint.ChangePassword), {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while changing password');
    }

    return response;
  };

  requestPasswordReset = async (email: string, captchaToken: string, lng: string | null = null): FetchResponse<undefined> => {
    Logger.Api.log('performing request password reset action');

    const response = await WrappedFetch<undefined>(this.api.getUrlWithQuery(Endpoint.RequestPasswordReset, { lng: lng }), {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while requesting password reset');
    }

    return response;
  };

  performResetPassword = async (code: string, password: string, captchaToken: string): FetchResponse<undefined> => {
    Logger.Api.log('performing reset password action');

    const response = await WrappedFetch<undefined>(this.api.getUrl(Endpoint.ResetPassword), {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while resetting password');
    }

    return response;
  };

  loadSettings = async (): FetchResponse<UserSettings> => {
    Logger.Api.log('loading user settings');

    const response = await WrappedFetch<UserSettings>(this.api.getUrl(Endpoint.UserSettings), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      requestOptions: {
        withCredentials: true
      }
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while loading settings');
    }

    return response;
  };

  storeSettings = async (settings: string): FetchResponse<undefined> => {
    Logger.Api.log('storing user settings');

    const response = await WrappedFetch<undefined>(this.api.getUrl(Endpoint.UserSettings), {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while loading settings');
    }

    return response;
  };

  performCreateAccount = async (data: CreateAccount, lng: string | null = null): FetchResponse<undefined> => {
    Logger.Api.log(`performing create account action of ${data.username} with ${data.email}`);

    const response = await WrappedFetch(this.api.getUrlWithQuery(Endpoint.CreateAccount, { lng: lng }), {
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
        code: data.code,
        acceptedTerms: data.acceptedTerms,
        captchaToken: data.captchaToken
      })
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while creating an account');
      return response;
    }

    return response;
  };

  performDeleteAccount = async (password: string): FetchResponse<undefined> => {
    Logger.Api.log('performing delete account action');

    const response = await WrappedFetch(this.api.getUrl(Endpoint.DeleteAccount), {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while deleting account');
      return response;
    }

    return response;
  };

  verifyAccount = async (id: string, code: string): FetchResponse<undefined> => {
    Logger.Api.log(`verifying account ${id} with code ${code}`);

    const response = await WrappedFetch(
      this.api.getDynamicUrl(Endpoint.AccountVerify, {
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

    if (response.isError) {
      Logger.Api.warn('cant verify account');
      return response;
    }

    return response;
  };

  resendVerificationEmail = async (email: string, lng: string | null = null): FetchResponse<undefined> => {
    Logger.Api.log(`resending verification email of ${email}`);

    const response = await WrappedFetch(this.api.getUrlWithQuery(Endpoint.ResendVerificationEmail, { lng: lng }), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        email: email
      })
    });

    if (response.isError) {
      Logger.Api.warn('cant resend verification email');
      return response;
    }

    return response;
  };
}
