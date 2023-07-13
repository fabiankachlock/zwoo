import { defineStore } from 'pinia';

import { getBackendErrorTranslation, unwrapBackendError } from '@/core/api/ApiError';
import { I18nInstance } from '@/i18n';

import { CaptchaValidator } from '../services/validator/captcha';
import { EmailValidator } from '../services/validator/email';
import { PasswordValidator } from '../services/validator/password';
import { PasswordMatchValidator } from '../services/validator/passwordMatch';
import { UsernameValidator } from '../services/validator/username';
import { useConfig, ZwooConfigKey } from './config';
import { useApi } from './helper/useApi';

export const useAuth = defineStore('auth', {
  state: () => {
    return {
      isInitialized: false,
      isLoggedIn: false,
      username: '',
      wins: -1
    };
  },
  actions: {
    async login(email: string, password: string, captchaResponse: string | undefined) {
      const captchaValid = new CaptchaValidator().validate(captchaResponse);
      if (!captchaValid.isValid) throw captchaValid.getErrors();

      const status = await useApi().loginUser({
        login: email,
        password: password,
        captchaToken: captchaResponse ?? ''
      });

      if (status.isLoggedIn) {
        this.$patch({
          username: status.username,
          isLoggedIn: status.isLoggedIn
        });
        useConfig().login();
      } else {
        this.isLoggedIn = false;
        const [, error] = unwrapBackendError(status);
        if (error) {
          throw error;
        }
      }
    },
    async logout() {
      const status = await useApi().logoutUser();
      if (!status.isLoggedIn) {
        const [, error] = unwrapBackendError(status);
        if (error) {
          throw getBackendErrorTranslation(error);
        }
      }
      useConfig().logout();
      this.$patch({
        username: '',
        isLoggedIn: status.isLoggedIn,
        wins: -1
      });
    },
    async createAccount(
      username: string,
      email: string,
      password: string,
      repeatPassword: string,
      captchaResponse: string | undefined,
      beta: string
    ) {
      const usernameValid = new UsernameValidator().validate(username);
      if (!usernameValid.isValid) throw usernameValid.getErrors();

      const emailValid = new EmailValidator().validate(email);
      if (!emailValid.isValid) throw emailValid.getErrors();

      const passwordValid = new PasswordValidator().validate(password);
      if (!passwordValid.isValid) throw passwordValid.getErrors();

      const passwordMatchValid = new PasswordMatchValidator().validate([password, repeatPassword]);
      if (!passwordMatchValid.isValid) throw passwordMatchValid.getErrors();

      const captchaValid = new CaptchaValidator().validate(captchaResponse);
      if (!captchaValid.isValid) throw captchaValid.getErrors();

      const status = await useApi().createUserAccount(
        {
          username,
          email,
          password,
          beta,
          captchaToken: captchaResponse ?? ''
        },
        useConfig().get(ZwooConfigKey.Language)
      );

      if (status.isLoggedIn) {
        this.$patch({
          username: status.username,
          isLoggedIn: status.isLoggedIn
        });
        this.askStatus();
      } else {
        this.isLoggedIn = false;
        const [, error] = unwrapBackendError(status);
        if (error) {
          throw getBackendErrorTranslation(error);
        }
      }
    },
    async deleteAccount(password: string) {
      const result = await useApi().deleteUserAccount(password);
      if (!result.isLoggedIn) {
        const [, error] = unwrapBackendError(result);
        if (error) {
          throw getBackendErrorTranslation(error);
        }
      }
      useConfig().logout();

      this.$patch({
        username: '',
        isLoggedIn: false
      });
    },
    async changePassword(oldPassword: string, newPassword: string, newPasswordRepeat: string) {
      const passwordValid = new PasswordValidator().validate(newPassword);
      if (!passwordValid.isValid) throw passwordValid.getErrors();

      const passwordMatchValid = new PasswordMatchValidator().validate([newPassword, newPasswordRepeat]);
      if (!passwordMatchValid.isValid) throw passwordMatchValid.getErrors();

      const response = await useApi().changeUserPassword(oldPassword, newPassword);

      if (response.error) {
        throw getBackendErrorTranslation(response.error);
      }
    },
    async requestPasswordReset(email: string, captchaResponse: string | undefined) {
      const emailValid = new EmailValidator().validate(email);
      if (!emailValid.isValid) throw emailValid.getErrors();

      const captchaValid = new CaptchaValidator().validate(captchaResponse);
      if (!captchaValid.isValid) throw captchaValid.getErrors();

      const response = await useApi().requestUserPasswordReset(email, captchaResponse ?? '', useConfig().get(ZwooConfigKey.Language));

      if (response.error) {
        throw getBackendErrorTranslation(response.error);
      }
    },
    async resetPassword(code: string, password: string, passwordRepeat: string, captchaResponse: string | undefined) {
      const passwordValid = new PasswordValidator().validate(password);
      if (!passwordValid.isValid) throw passwordValid.getErrors();

      const passwordMatchValid = new PasswordMatchValidator().validate([password, passwordRepeat]);
      if (!passwordMatchValid.isValid) throw passwordMatchValid.getErrors();

      const captchaValid = new CaptchaValidator().validate(captchaResponse);
      if (!captchaValid.isValid) throw captchaValid.getErrors();

      const response = await useApi().resetUserPassword(code, password, captchaResponse ?? '');

      if (response.error) {
        throw getBackendErrorTranslation(response.error);
      }
    },
    async askStatus() {
      const response = await useApi().loadUserInfo();
      if (response.isLoggedIn) {
        this.$patch({
          username: response.username,
          isLoggedIn: response.isLoggedIn,
          wins: response.wins ?? -1,
          isInitialized: true
        });
      } else {
        this.$patch({
          isLoggedIn: response.isLoggedIn,
          isInitialized: true
        });
      }
    },
    async configure() {
      await this.askStatus();
      // setup initial config
      if (this.isLoggedIn) {
        useConfig().login();
      }
    },
    async __FIX_resolveNameAsync(): Promise<string> {
      const username = I18nInstance.t('offline.playerName');
      if (username === 'offline.playerName') {
        // OR I18nInstance.getLocaleMessage(I18nInstance.locale.value) === {}
        await new Promise(res => setTimeout(() => res({}), 200));
        return this.__FIX_resolveNameAsync();
      }
      return username;
    },
    applyOfflineConfig() {
      this.__FIX_resolveNameAsync().then(username => {
        this.$patch({
          isInitialized: true,
          isLoggedIn: true,
          username: username,
          wins: 0
        });
      });
    }
  }
});
