import semverRCompare from 'semver/functions/rcompare';

import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { BackendErrorAble } from '../ApiError';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class ConfigService {
  static checkVersion = async (version: string, zrp: string): Promise<BackendErrorAble<boolean>> => {
    Logger.Api.log(`fetching version`);

    const response = await WrappedFetch(`${Backend.getUrl(Endpoint.Discover)}?t=${Date.now()}`, {
      useBackend: AppConfig.UseBackend,
      fallbackValue: true,
      method: 'POST',
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        version: version,
        zrpVersion: zrp
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while fetching version');
      return {
        error: response.error
      };
    }

    return true;
  };

  static fetchVersionHistory = async (): Promise<BackendErrorAble<string[]>> => {
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
  };

  static fetchChangelog = async (version: string): Promise<BackendErrorAble<string>> => {
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
  };
}
