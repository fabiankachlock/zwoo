import semverRCompare from 'semver/functions/rcompare';

import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { FetchResponse } from '../ApiEntities';
import { ClientInfo, VersionHistory } from '../entities/Misc';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class ConfigService {
  static checkVersion = async (version: string, zrp: string): FetchResponse<ClientInfo> => {
    Logger.Api.log(`fetching version`);

    const response = await WrappedFetch<ClientInfo>(`${Backend.getUrl(Endpoint.Discover)}?t=${Date.now()}`, {
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        version: AppConfig.Version,
        zrpVersion: '' // TODO: use real zrp version
      },
      method: 'POST',
      responseOptions: {
        decodeJson: false
      },
      body: JSON.stringify({
        version: version,
        zrpVersion: zrp
      })
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while fetching version');
      return response;
    }

    return response;
  };

  static fetchVersionHistory = async (): FetchResponse<VersionHistory> => {
    Logger.Api.log(`fetching version history`);

    const response = await WrappedFetch<{ versions: string[] }>(Backend.getUrl(Endpoint.VersionHistory), {
      useBackend: AppConfig.UseBackend,
      fallbackValue: { versions: ['v1.0.0'] }
    });

    if (response.isError) {
      Logger.Api.warn(`received erroneous response while fetching version history`);
      return response;
    }
    return {
      ...response,
      data: {
        versions: (response.data?.versions ?? []).sort(semverRCompare)
      }
    };
  };

  static fetchChangelog = async (version: string): FetchResponse<string> => {
    Logger.Api.log(`fetching changelog for ${version}`);

    const response = await WrappedFetch<string>(Backend.getDynamicUrl(Endpoint.Changelog, { version: version }), {
      useBackend: AppConfig.UseBackend,
      fallbackValue: '<h1>Dev Build</h1>',
      responseOptions: {
        decodeJson: false
      }
    });

    if (response.isError) {
      Logger.Api.warn(`received erroneous response while fetching changelog ${version}`);
      return response;
    }
    return response;
  };
}
