import { defineStore } from 'pinia';

type CookiesConfig = {
  recaptcha: boolean;
};

const PreferredSelection: CookiesConfig = {
  recaptcha: true
};

const DefaultSelection: CookiesConfig = {
  recaptcha: false
};

export const useCookies = defineStore('cookies', {
  state: () => ({
    popupShowed: false,
    cookies: DefaultSelection
  }),

  getters: {
    recaptchaCookie: state => (state.popupShowed ? state.cookies.recaptcha : PreferredSelection.recaptcha)
  },

  actions: {
    setReCaptchaCookie(allowed: boolean) {
      this.cookies.recaptcha = allowed;
    },
    acceptAll() {
      this.cookies.recaptcha = true;
    },
    didShowDialog() {
      this.popupShowed = true;
    }
  }
});
