import { defineStore } from 'pinia';
import { AuthenticationService } from '../services/api/Authentication';
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

      this.$patch({
        username: status.username,
        isLoggedIn: status.isLoggedIn
      });
    },
    async logout() {
      const status = await AuthenticationService.performLogout();

      this.$patch({
        username: status.username,
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

      this.$patch({
        username: status.username,
        isLoggedIn: status.isLoggedIn
      });
    },
    async askStatus() {
      // make initial api call (to read from session)
    },
    async configure() {
      this.askStatus();
    }
  }
});
