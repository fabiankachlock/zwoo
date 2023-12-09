import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { FetchResponse } from '../ApiEntities';
import { ContactForm } from '../entities/ContactForm';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class MiscApiService {
  static submitContactForm = async (data: ContactForm): FetchResponse<void> => {
    Logger.Api.log(`submitting contact form`);

    const response = await WrappedFetch(`${Backend.getUrl(Endpoint.ContactFormSubmission)}`, {
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

    if (response.isError) {
      Logger.Api.warn('received erroneous response while submitting contact form');
      return response;
    }
    return response;
  };
}
