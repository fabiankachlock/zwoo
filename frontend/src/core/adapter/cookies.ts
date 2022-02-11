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

const cookiesKey = 'zwoo:cookies';

const saveCookies = (selection: CookiesConfig) => {
  const json = JSON.stringify(selection);
  let base64 = json;
  for (let i = 0; i < 5; i++) {
    base64 = btoa(base64);
  }
  localStorage.setItem(cookiesKey, base64);
};

const readCookies = (): CookiesConfig | undefined => {
  const stored = localStorage.getItem(cookiesKey);
  if (!stored) return undefined;
  let json = stored;
  for (let i = 0; i < 5; i++) {
    json = atob(json);
  }
  return JSON.parse(json);
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
    setup() {
      try {
        const selection = readCookies();
        if (selection) {
          this.popupShowed = true;
          this.cookies = {
            ...DefaultSelection,
            ...selection
          };
        }
      } catch (_e: unknown) {
        // invalid config
        this.popupShowed = false;
        localStorage.removeItem(cookiesKey);
      }
    },
    setReCaptchaCookie(allowed: boolean) {
      this.cookies.recaptcha = allowed;
    },
    acceptAll() {
      this.cookies.recaptcha = true;
    },
    didShowDialog() {
      saveCookies(this.cookies);
      this.popupShowed = true;
    }
  }
});
