import { defineStore } from 'pinia';

import { AccountService } from '../api/restapi/Account';
import { AuthenticationService } from '../api/restapi/Authentication';
import { ReCaptchaResponse } from '../api/restapi/Captcha';
import { getBackendErrorTranslation, unwrapBackendError } from '../api/restapi/Errors';
import { EmailValidator } from '../services/validator/email';
import { PasswordValidator } from '../services/validator/password';
import { PasswordMatchValidator } from '../services/validator/passwordMatch';
import { RecaptchaValidator } from '../services/validator/recaptcha';
import { UsernameValidator } from '../services/validator/username';
import { useConfig, ZwooConfigKey } from './config';

export const useAuth = defineStore('auth', {
  state: () => {
    return {
      isInitialized: false,
      isLoggedIn: false,
      username: '',
      publicId: '', // TODO: dont hardcode this,
      wins: -1
    };
  },
  actions: {
    async login(email: string, password: string, recaptchaResponse: ReCaptchaResponse | undefined) {
      const recaptchaValid = new RecaptchaValidator().validate(recaptchaResponse);
      if (!recaptchaValid.isValid) throw recaptchaValid.getErrors();

      const status = await AuthenticationService.performLogin(email, password);

      if (status.isLoggedIn) {
        this.$patch({
          username: status.username,
          publicId: `p_${status.username}`,
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
      const status = await AuthenticationService.performLogout();
      if (!status.isLoggedIn) {
        const [, error] = unwrapBackendError(status);
        if (error) {
          throw getBackendErrorTranslation(error);
        }
      }
      useConfig().logout();
      this.$patch({
        username: '',
        publicId: '',
        isLoggedIn: status.isLoggedIn,
        wins: -1
      });
    },
    async createAccount(
      username: string,
      email: string,
      password: string,
      repeatPassword: string,
      recaptchaResponse: ReCaptchaResponse | undefined,
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

      const recaptchaValid = new RecaptchaValidator().validate(recaptchaResponse);
      if (!recaptchaValid.isValid) throw recaptchaValid.getErrors();

      const status = await AuthenticationService.performCreateAccount(username, email, password, beta, useConfig().get(ZwooConfigKey.Language));

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
      const result = await AuthenticationService.performDeleteAccount(password);
      if (!result.isLoggedIn) {
        const [, error] = unwrapBackendError(result);
        if (error) {
          throw getBackendErrorTranslation(error);
        }
      }
      useConfig().logout();

      this.$patch({
        username: '',
        publicId: '',
        isLoggedIn: false
      });
    },
    async changePassword(oldPassword: string, newPassword: string, newPasswordRepeat: string) {
      const passwordValid = new PasswordValidator().validate(newPassword);
      if (!passwordValid.isValid) throw passwordValid.getErrors();

      const passwordMatchValid = new PasswordMatchValidator().validate([newPassword, newPasswordRepeat]);
      if (!passwordMatchValid.isValid) throw passwordMatchValid.getErrors();

      const response = await AccountService.performChangePassword(oldPassword, newPassword);

      if (response.error) {
        throw getBackendErrorTranslation(response.error);
      }
    },
    async requestPasswordReset(email: string, recaptchaResponse: ReCaptchaResponse | undefined) {
      const emailValid = new EmailValidator().validate(email);
      if (!emailValid.isValid) throw emailValid.getErrors();

      const recaptchaValid = new RecaptchaValidator().validate(recaptchaResponse);
      if (!recaptchaValid.isValid) throw recaptchaValid.getErrors();

      const response = await AccountService.requestPasswordReset(email, useConfig().get(ZwooConfigKey.Language));

      if (response.error) {
        throw getBackendErrorTranslation(response.error);
      }
    },
    async resetPassword(code: string, password: string, passwordRepeat: string, recaptchaResponse: ReCaptchaResponse | undefined) {
      const passwordValid = new PasswordValidator().validate(password);
      if (!passwordValid.isValid) throw passwordValid.getErrors();

      const passwordMatchValid = new PasswordMatchValidator().validate([password, passwordRepeat]);
      if (!passwordMatchValid.isValid) throw passwordMatchValid.getErrors();

      const recaptchaValid = new RecaptchaValidator().validate(recaptchaResponse);
      if (!recaptchaValid.isValid) throw recaptchaValid.getErrors();

      const response = await AccountService.performResetPassword(code, password);

      if (response.error) {
        throw getBackendErrorTranslation(response.error);
      }
    },
    async askStatus() {
      const response = await AuthenticationService.getUserInfo();
      if (response.isLoggedIn) {
        this.$patch({
          username: response.username,
          publicId: `p_${response.username}`,
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
    }
  }
});
