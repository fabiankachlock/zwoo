import Logger from '../logging/logImport';
import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble, parseBackendError } from './Errors';

export type LeaderBoardResponse = {
  leaderboard: {
    username: string;
    wins: number;
  }[];
};

export type LeaderBoardPositionResponse = {
  position: number;
};

export class LeaderBoardService {
  static async fetchLeaderBoard(): Promise<BackendErrorAble<LeaderBoardResponse>> {
    Logger.Api.log(`fetching leaderboard`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking leaderboard response');
      return Promise.resolve({
        leaderboard: new Array(50).fill(null).map((_, index) => ({
          username: `user-${index}`,
          wins: 50 - index
        }))
      });
    }

    const req = await fetch(Backend.getUrl(Endpoint.LeaderBoard));

    if (req.status != 200) {
      Logger.Api.warn('received erroneous response while fetching leaderboard');
      return {
        error: parseBackendError(await req.text())
      };
    }

    const response = (await req.json()) as LeaderBoardResponse;
    return response;
  }

  static async fetchOwnLeaderBoardPosition(): Promise<BackendErrorAble<LeaderBoardPositionResponse>> {
    Logger.Api.log(`fetching own leaderboard position`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking own leaderboard position response');
      return Promise.resolve({
        position: 1
      });
    }

    const req = await fetch(Backend.getUrl(Endpoint.LeaderBoardPosition), {
      method: 'GET',
      credentials: 'include'
    });

    if (req.status != 200) {
      Logger.Api.warn('received erroneous response while fetching own leaderboard position');
      return {
        error: parseBackendError(await req.text())
      };
    }

    const response = (await req.json()) as LeaderBoardPositionResponse;
    return response;
  }
}
