import { AppConfig } from '@/config';
import Logger from '@/core/services/logging/logImport';

import { BackendErrorAble } from '../ApiError';
import { LeaderBoardPositionResponse, LeaderBoardResponse } from '../entities/Leaderboard';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class LeaderBoardService {
  static fetchLeaderBoard = async (): Promise<BackendErrorAble<LeaderBoardResponse>> => {
    Logger.Api.log(`fetching leaderboard`);

    const response = await WrappedFetch<LeaderBoardResponse>(Backend.getUrl(Endpoint.LeaderBoard), {
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        leaderboard: new Array(50).fill(null).map((_, index) => ({
          username: `user-${index}`,
          wins: 50 - index
        }))
      }
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while fetching leaderboard');
      return {
        error: response.error
      };
    }

    return response.data!;
  };

  static fetchOwnLeaderBoardPosition = async (): Promise<BackendErrorAble<LeaderBoardPositionResponse>> => {
    Logger.Api.log(`fetching own leaderboard position`);

    const response = await WrappedFetch<LeaderBoardPositionResponse>(Backend.getUrl(Endpoint.LeaderBoardPosition), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        position: 1
      },
      requestOptions: {
        withCredentials: true
      }
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while fetching own leaderboard position');
      return {
        error: response.error
      };
    }

    return response.data!;
  };
}
