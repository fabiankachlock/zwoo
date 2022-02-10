import { defineStore } from 'pinia';

export const useCookies = defineStore('cookies', {
  state: () => ({
    popupShowed: false,
    recaptchaAllowed: false
  }),

  actions: {
    setReCaptchaCookie(allowed: boolean) {
      this.recaptchaAllowed = allowed;
    },
    didShowDialog() {
      this.popupShowed = true;
    }
  }
});
