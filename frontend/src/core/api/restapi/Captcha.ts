import { AppConfig } from '@/config';
import { useRuntimeConfig } from '@/core/runtimeConfig';
import { ReCaptchaTermsVisibilityManager } from '@/router/guards/ReCaptchaTerms';

import { BackendErrorAble, parseBackendError, unwrapBackendError } from '../ApiError';
import { CaptchaResponse } from '../entities/Captcha';
import { Backend, Endpoint } from './ApiConfig';

/**
 * The Captcha Service is the only API Service which is not offline safe.
 * This is because in offline mode all activity which would require a captcha is disabled
 */
export class ReCaptchaService {
  private static instance = new ReCaptchaService();

  private isReady = false;

  private loaded = false;

  private whenLoaded = () => {
    ReCaptchaTermsVisibilityManager.init();
    ReCaptchaTermsVisibilityManager.updateState();
    this.isReady = true;
  };

  private loadScript = async () => {
    if (this.loaded) return;
    this.loaded = true;
    const siteKey = await useRuntimeConfig().recaptchaKey;
    const scriptTag = document.createElement('script');
    scriptTag.setAttribute('src', 'https://www.google.com/recaptcha/api.js?render=' + siteKey);
    document.body.appendChild(scriptTag);

    scriptTag.onload = () => {
      grecaptcha.ready(() => this.whenLoaded());
    };
  };

  private performCheck = async (): Promise<CaptchaResponse | undefined> => {
    if (!AppConfig.UseBackend) {
      return Promise.resolve({
        success: true,
        score: 1
      });
    }

    if (this.isReady) {
      const siteKey = await useRuntimeConfig().recaptchaKey;
      const token = await grecaptcha.execute(siteKey, {
        action: 'login'
      });
      const [result] = unwrapBackendError(await this.verify(token));
      return result;
    }
    return Promise.resolve({
      success: true,
      score: 1
    });
  };

  private verify = async (token: string): Promise<BackendErrorAble<CaptchaResponse>> => {
    if (AppConfig.UseBackend) {
      const req = await fetch(Backend.getUrl(Endpoint.Recaptcha), {
        method: 'POST',
        body: token
      });

      if (req.status !== 200) {
        return {
          error: parseBackendError(await req.text())
        };
      }

      const response = (await req.json()) as CaptchaResponse;
      return {
        success: response.success,
        score: response.score
      };
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