import { AppConfig } from '@/config';

import { BackendErrorAble } from '../ApiError';
import { CaptchaResponse } from '../entities/Captcha';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class CaptchaApiService {
  static verify = async (token: string): Promise<BackendErrorAble<CaptchaResponse>> => {
    const response = await WrappedFetch<CaptchaResponse>(Backend.getUrl(Endpoint.Recaptcha), {
      useBackend: AppConfig.UseBackend,
      method: 'POST',
      body: token,
      requestOptions: {
        contentType: 'text/plain'
      },
      fallbackValue: {
        score: 1,
        success: true
      }
    });

    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    return response.data!;
  };
}
