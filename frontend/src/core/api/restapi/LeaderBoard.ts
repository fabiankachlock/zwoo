import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { FetchResponse } from '../ApiEntities';
import { Leaderboard, LeaderboardPosition } from '../entities/Game';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class LeaderBoardService {
  private readonly api: Backend;

  public constructor(api: Backend) {
    this.api = api;
  }

  fetchLeaderBoard = async (): FetchResponse<Leaderboard> => {
    Logger.Api.log(`fetching leaderboard`);

    const response = await WrappedFetch<Leaderboard>(this.api.getUrl(Endpoint.LeaderBoard), {
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        leaderboard: new Array(50).fill(null).map((_, index) => ({
          username: `user-${index}`,
          wins: 50 - index
        }))
      }
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while fetching leaderboard');
      return response;
    }

    return response;
  };

  fetchOwnLeaderBoardPosition = async (): FetchResponse<LeaderboardPosition> => {
    Logger.Api.log(`fetching own leaderboard position`);

    const response = await WrappedFetch<LeaderboardPosition>(this.api.getUrl(Endpoint.LeaderBoardPosition), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        position: 1
      },
      requestOptions: {
        withCredentials: true
      }
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while fetching own leaderboard position');
      return response;
    }

    return response;
  };
}
