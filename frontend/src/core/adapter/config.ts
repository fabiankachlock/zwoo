import i18n, { defaultLanguage } from '@/i18n';
import { defineStore } from 'pinia';
import { AuthenticationService } from '../services/api/Authentication';
import { EmailValidator } from '../services/validator/email';
import { PasswordValidator } from '../services/validator/password';
import { PasswordMatchValidator } from '../services/validator/passwordMatch';
import Router from '@/router';
import { UsernameValidator } from '../services/validator/username';

const languageKey = 'zwoo:lng';
const uiKey = 'zwoo:ui';

const changeLanguage = (lng: string) => {
  i18n.global.locale.value = lng;
  localStorage.setItem(languageKey, lng);
};

const changeUIMode = (isDark: boolean) => {
  localStorage.setItem(uiKey, isDark ? 'dark' : 'light');
  if (isDark) {
    document.body.classList.add('dark');
  } else {
    document.body.classList.remove('dark');
  }
};

export const useConfig = defineStore('config', {
  state: () => {
    return {
      useDarkMode: false,
      language: 'en',
      username: '',
      isLoggedIn: false
    };
  },

  actions: {
    setDarkMode(isEnabled: boolean) {
      this.useDarkMode = isEnabled;
      changeUIMode(isEnabled);
    },
    setLanguage(lng: string) {
      this.language = lng;
      changeLanguage(lng);
    },
    async login(username: string, password: string) {
      const status = await AuthenticationService.performLogin(username, password);

      this.$patch({
        username: status.username,
        isLoggedIn: status.isLoggedIn
      });

      Router.push('/home');
    },
    async logout() {
      const status = await AuthenticationService.performLogout();

      this.$patch({
        username: status.username,
        isLoggedIn: status.isLoggedIn
      });

      Router.push('/landing');
    },
    async createAccount(username: string, email: string, password: string, repeatPassword: string) {
      const usernameValid = new UsernameValidator().validate(username);
      if (!usernameValid.isValid) throw usernameValid.getErrors();

      const emailValid = new EmailValidator().validate(email);
      if (!emailValid.isValid) throw emailValid.getErrors();

      const passwordValid = new PasswordValidator().validate(password);
      if (!passwordValid.isValid) throw passwordValid.getErrors();

      const passwordMatchValid = new PasswordMatchValidator().validate([password, repeatPassword]);
      if (!passwordMatchValid.isValid) throw passwordMatchValid.getErrors();

      const status = await AuthenticationService.performCreateAccount(username, email, password);

      this.$patch({
        username: status.username,
        isLoggedIn: status.isLoggedIn
      });

      Router.push('/home');
    },
    configure() {
      const storedLng = localStorage.getItem(languageKey) || defaultLanguage;
      i18n.global.locale.value = storedLng;

      const rawStoredDarkMode = localStorage.getItem(uiKey);
      const storedDarkMode = rawStoredDarkMode
        ? rawStoredDarkMode === 'dark'
        : window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
      changeUIMode(storedDarkMode);

      this.$patch({
        useDarkMode: storedDarkMode,
        language: storedLng
      });
    }
  }
});
