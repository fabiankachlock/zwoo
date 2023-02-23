import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble } from './Errors';
import { WrappedFetch } from './FetchWrapper';

export class MiscApiService {
  static async submitContactForm(sender: string, message: string): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`submitting contact form`);

    const response = await WrappedFetch<string>(`${Backend.getUrl(Endpoint.ContactFormSubmission)}`, {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        sender: sender,
        message: message
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while submitting contact form');
      return {
        error: response.error
      };
    }

    return response.data ?? '';
  }
}
