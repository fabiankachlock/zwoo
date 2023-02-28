import { AppConfig } from '@/config';

import { BackendErrorAble, parseBackendError } from '../ApiError';
import { CaptchaResponse } from '../entities/Captcha';
import { Backend, Endpoint } from './ApiConfig';

export class CaptchaApiService {
  static verify = async (token: string): Promise<BackendErrorAble<CaptchaResponse>> => {
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
}
