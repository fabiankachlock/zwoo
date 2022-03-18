import { ReCaptchaTermsVisibilityManager } from '../security/ReCaptchaTerms';
import { Backend, Endpoint } from './apiConfig';

export type ReCaptchaResponse = {
  success: boolean;
  score: number;
};

export class ReCaptchaService {
  static SITE_KEY = '6LdVYE0dAAAAAPkDshZhPtnlqklX8fCH_2xMJvsm';

  private static instance = new ReCaptchaService();

  private isReady = false;

  private loaded = false;

  private whenLoaded = () => {
    ReCaptchaTermsVisibilityManager.init();
    ReCaptchaTermsVisibilityManager.updateState();
    this.isReady = true;
  };

  private loadScript = () => {
    if (this.loaded) return;
    this.loaded = true;
    const scriptTag = document.createElement('script');
    scriptTag.setAttribute('src', 'https://www.google.com/recaptcha/api.js?render=' + ReCaptchaService.SITE_KEY);
    document.body.appendChild(scriptTag);

    scriptTag.onload = () => {
      grecaptcha.ready(() => this.whenLoaded());
    };
  };

  private performCheck = async (): Promise<ReCaptchaResponse | undefined> => {
    if (this.isReady) {
      const token = await grecaptcha.execute(ReCaptchaService.SITE_KEY, {
        action: 'login'
      });
      return this.verify(token);
    }
    if (process.env.VUE_APP_USE_BACKEND === 'true') {
      return Promise.resolve(undefined);
    }
    return Promise.resolve({
      success: true,
      score: 1
    });
  };

  private verify = async (token: string): Promise<ReCaptchaResponse> => {
    if (process.env.VUE_APP_USE_BACKEND === 'true') {
      return fetch(Backend.getUrl(Endpoint.Recaptcha), {
        method: 'POST',
        body: token
      })
        .then(res => res.json() as Promise<ReCaptchaResponse>)
        .then(res => ({
          success: res.success,
          score: res.score
        }));
    }
    return Promise.resolve({
      success: true,
      score: 1
    });
  };

  static checkUser = ReCaptchaService.instance.performCheck;

  static load = ReCaptchaService.instance.loadScript;
}

export default ReCaptchaService;
