import { defineStore } from 'pinia';
import { AuthenticationService } from '../services/api/Authentication';
import { getBackendErrorTranslation, unwrapBackendError } from '../services/api/errors';
import { ReCaptchaResponse } from '../services/api/reCAPTCHA';
import { EmailValidator } from '../services/validator/email';
import { PasswordValidator } from '../services/validator/password';
import { PasswordMatchValidator } from '../services/validator/passwordMatch';
import { RecaptchaValidator } from '../services/validator/recaptcha';
import { UsernameValidator } from '../services/validator/username';

export const useAuth = defineStore('auth', {
  state: () => {
    return {
      isInitialized: false,
      isLoggedIn: false,
      isAdmin: false,
      username: '',
      userId: ''
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
          isLoggedIn: status.isLoggedIn
        });
      } else {
        this.isLoggedIn = false;
        const [, error] = unwrapBackendError(status);
        if (error) {
          throw getBackendErrorTranslation(error);
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

      this.$patch({
        username: '',
        isLoggedIn: status.isLoggedIn
      });
    },
    async createAccount(username: string, email: string, password: string, repeatPassword: string, recaptchaResponse: ReCaptchaResponse | undefined) {
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

      const status = await AuthenticationService.performCreateAccount(username, email, password);

      if (status.isLoggedIn) {
        this.$patch({
          username: status.username,
          isLoggedIn: status.isLoggedIn
        });
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

      this.$patch({
        username: '',
        isLoggedIn: false
      });
    },
    async askStatus() {
      const response = await AuthenticationService.getUserInfo();
      if (response.isLoggedIn) {
        this.$patch({
          username: response.username,
          isLoggedIn: response.isLoggedIn,
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
      this.askStatus();
    }
  }
});
