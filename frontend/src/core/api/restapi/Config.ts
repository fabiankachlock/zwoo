import semverRCompare from 'semver/functions/rcompare';

import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble } from './Errors';
import { WrappedFetch } from './FetchWrapper';

export class ConfigService {
  static async fetchVersion(): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching version`);

    const response = await WrappedFetch<string>(`${Backend.getUrl(Endpoint.Version)}?t=${Date.now()}`, {
      useBackend: AppConfig.UseBackend,
      fallbackValue: AppConfig.Version,
      responseOptions: {
        decodeJson: false
      }
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while fetching version');
      return {
        error: response.error
      };
    }

    return response.data!;
  }

  static async fetchVersionHistory(): Promise<BackendErrorAble<string[]>> {
    Logger.Api.log(`fetching version history`);

    const response = await WrappedFetch<{ versions: string[] }>(Backend.getUrl(Endpoint.VersionHistory), {
      useBackend: AppConfig.UseBackend,
      fallbackValue: { versions: ['v1.0.0'] }
    });

    if (response.error) {
      Logger.Api.warn(`received erroneous response while fetching version history`);
      return {
        error: response.error
      };
    }
    return (response.data?.versions ?? []).sort(semverRCompare);
  }

  static async fetchChangelog(version: string): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching changelog for ${version}`);

    const response = await WrappedFetch<string>(Backend.getDynamicUrl(Endpoint.Changelog, { version: version }), {
      useBackend: AppConfig.UseBackend,
      fallbackValue: '<h1>Dev Build</h1>',
      responseOptions: {
        decodeJson: false
      }
    });

    if (response.error) {
      Logger.Api.warn(`received erroneous response while fetching changelog ${version}`);
      return {
        error: response.error
      };
    }
    return response.data ?? `<h2><i>No changes in version ${version}</i></h2>`;
  }
}
