import semverRCompare from 'semver/functions/rcompare';

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

  static async fetchVersionHistory(): Promise<BackendErrorAble<string[]>> {
    Logger.Api.log(`fetching version history`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking version history response');
      return ['v1.0.0', 'v1.0.0-beta', 'v1.0.0-alpha'];
    }

    const req = await fetch(Backend.getUrl(Endpoint.VersionHistory));

    if (req.status !== 200) {
      Logger.Api.warn(`received erroneous response while fetching version history`);
      return {
        error: parseBackendError(await req.text())
      };
    }
    return ((await (req.json() as Promise<{ versions: string[] }>)).versions ?? []).sort(semverRCompare);
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
