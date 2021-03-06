import Logger from '../logging/logImport';
import { Backend, Endpoint } from './apiConfig';
import { BackendErrorAble, parseBackendError } from './errors';

export class ConfigService {
  static async fetchVersion(): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching version`);
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking version response');
      return process.env.VUE_APP_VERSION;
    }

    const req = await fetch(Backend.getUrl(Endpoint.Version));

    if (req.status != 200) {
      Logger.Api.warn('received erroneous response while fetching version');
      return {
        error: parseBackendError(await req.text())
      };
    }

    return await req.text();
  }
}
