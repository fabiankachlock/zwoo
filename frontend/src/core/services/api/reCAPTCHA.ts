export type ReCaptchaResponse = {
  success: boolean;
  score: number;
};

class ReadyState {
  constructor(public ready: boolean = false) {}

  setReady = () => {
    this.ready = true;
  };
}

export class ReCaptchaService {
  static SITE_KEY = '6LdVYE0dAAAAAPkDshZhPtnlqklX8fCH_2xMJvsm';

  private static state = (() => {
    grecaptcha.ready(() => ReCaptchaService.state.setReady());
    return new ReadyState();
  })();

  static performCheck = async (): Promise<ReCaptchaResponse | undefined> => {
    if (this.state.ready) {
      const token = await grecaptcha.execute(ReCaptchaService.SITE_KEY, {
        action: 'login'
      });
      return ReCaptchaService.verify(token);
    }
    return Promise.resolve(undefined);
  };

  static verify = async (token: string): Promise<ReCaptchaResponse> => {
    return fetch('/verify', {
      method: 'POST',
      body: JSON.stringify({
        token: token
      })
    })
      .then(res => res.json() as Promise<ReCaptchaResponse>)
      .then(res => res); // TODO: optional data casting;
  };
}
