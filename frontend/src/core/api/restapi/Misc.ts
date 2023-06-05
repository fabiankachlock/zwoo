import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { BackendErrorAble } from '../ApiError';
import { ContactForm } from '../entities/ContactForm';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class MiscApiService {
  static submitContactForm = async (data: ContactForm): Promise<BackendErrorAble<string>> => {
    Logger.Api.log(`submitting contact form`);

    const response = await WrappedFetch<string>(`${Backend.getUrl(Endpoint.ContactFormSubmission)}`, {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        name: data.name,
        email: data.email,
        message: data.message,
        captchaToken: data.captchaToken,
        site: data.site
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while submitting contact form');
      return {
        error: response.error
      };
    }

    return response.data ?? '';
  };
}
