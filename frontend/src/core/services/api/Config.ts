import Logger from '../logging/logImport';
import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble, parseBackendError } from './Errors';

export class ConfigService {
  static async fetchVersion(): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching version`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking version response');
      return import.meta.env.VUE_APP_VERSION;
    }

    const req = await fetch(`${Backend.getUrl(Endpoint.Version)}?t=${Date.now()}`);

    if (req.status !== 200) {
      Logger.Api.warn('received erroneous response while fetching version');
      return {
        error: parseBackendError(await req.text())
      };
    }

    return await req.text();
  }

  static async fetchChangelog(version: string): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching changelog for ${version}`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking changelog response');
      return `<h2><i>No changes in version ${version}</i></h2>`;
    }

    const req = await fetch(Backend.getDynamicUrl(Endpoint.Changelog, { version: version }));

    if (req.status !== 200) {
      Logger.Api.warn(`received erroneous response while fetching changelog ${version}`);
      return {
        error: parseBackendError(await req.text())
      };
    }
    return await req.text();
  }
}
