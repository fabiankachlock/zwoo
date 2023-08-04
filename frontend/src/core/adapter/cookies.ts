import { defineStore } from 'pinia';

type CookiesConfig = {
  // recaptcha: boolean;
};

const DefaultSelection: CookiesConfig = {
  // recaptcha: false !Important: TTDSG §25 -> active aception of optional cookies needed
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

// const readCookies = (): CookiesConfig | undefined => {
//   const stored = localStorage.getItem(cookiesKey);
//   if (!stored) return undefined;
//   let json = stored;
//   for (let i = 0; i < 5; i++) {
//     json = atob(json);
//   }
//   return JSON.parse(json);
// };

export const useCookies = defineStore('cookies', {
  state: () => ({
    popupShowed: true,
    popupOpen: false,
    cookies: DefaultSelection
  }),

  getters: {
    // recaptchaCookie: state => (state.popupShowed ? state.cookies.recaptcha : PreferredSelection.recaptcha)
  },

  actions: {
    setup() {
      // try {
      //   const selection = readCookies();
      //   if (selection) {
      //     this.popupShowed = true;
      //     this.cookies = {
      //       ...DefaultSelection,
      //       ...selection
      //     };
      //   } else {
      //     this.popupOpen = true;
      //   }
      // } catch (_e: unknown) {
      //   // invalid config
      //   this.popupShowed = false;
      //   this.popupOpen = true;
      //   localStorage.removeItem(cookiesKey);
      // }
    },
    // setReCaptchaCookie(allowed: boolean) {
    //   this.cookies.recaptcha = allowed;
    // },
    rejectAll() {
      // this.cookies.recaptcha = false;
    },
    async loadRecaptcha() {
      // if (this.cookies.recaptcha) {
      //   const reCaptchaService = await import('./captcha');
      //   reCaptchaService.useCaptcha()._loadScript();
      // }
    },
    didShowDialog() {
      saveCookies(this.cookies);
      this.popupShowed = true;
      this.popupOpen = false;
    }
  }
});
