import { ReCaptchaTermsVisibilityManager } from '../security/ReCaptchaTerms';

export type ReCaptchaResponse = {
  success: boolean;
  score: number;
};

export class ReCaptchaService {
  static SITE_KEY = '6LdVYE0dAAAAAPkDshZhPtnlqklX8fCH_2xMJvsm';

  private static instance = new ReCaptchaService();

  private isReady = false;

  private whenLoaded = () => {
    ReCaptchaTermsVisibilityManager.init();
    ReCaptchaTermsVisibilityManager.updateState();
    this.isReady = true;
  };

  private loadScript = () => {
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
    return Promise.resolve(undefined);
  };

  private verify = async (token: string): Promise<ReCaptchaResponse> => {
    // return fetch('/verify', {
    //   method: 'POST',
    //   body: JSON.stringify({
    //     token: token
    //   })
    // })
    //   .then(res => res.json() as Promise<ReCaptchaResponse>)
    //   .then(res => res); // TODO: optional data casting;
    console.log(token);
    return Promise.resolve({
      success: Math.random() > 0.5,
      score: Math.random()
    });
  };

  static checkUser = ReCaptchaService.instance.performCheck;

  static load = ReCaptchaService.instance.loadScript;
}

export default ReCaptchaService;
