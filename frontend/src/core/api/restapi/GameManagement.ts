import { AppConfig } from '@/config';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import Logger from '@/core/services/logging/logImport';

import { FetchResponse } from '../ApiEntities';
import { GameJoinResponse, GameMeta, GamesList } from '../entities/Game';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): FetchResponse<GameJoinResponse> => {
    Logger.Api.log(`creating ${isPublic ? 'public' : 'non-public'} game ${name}`);
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking create game response');
    }

    const response = await WrappedFetch<{ gameId: number; ownId: number; isRunning: boolean; role: ZRPRole }>(Backend.getUrl(Endpoint.CreateGame), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      fallbackValue: { gameId: 1, ownId: 2, isRunning: false, role: ZRPRole.Host },
      requestOptions: {
        withCredentials: true
      },
      body: JSON.stringify({
        name: name,
        password: password,
        usePassword: !isPublic
      })
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while creating game');
      return response;
    }

    return response;
  };

  static listAll = async (): FetchResponse<GamesList> => {
    Logger.Api.log('fetching all games');

    const response = await WrappedFetch<GamesList>(
      Backend.getDynamicUrl(Endpoint.Games, {
        filter: '',
        limit: '100',
        offset: '0',
        public: 'false',
        recommended: 'false'
      }),
      {
        method: 'GET',
        useBackend: AppConfig.UseBackend,
        fallbackValue: {
          games: [
            {
              id: 1,
              isPublic: true,
              name: 'DEV',
              playerCount: -1
            }
          ]
        },
        requestOptions: {
          withCredentials: true
        }
      }
    );

    if (response.isError) {
      Logger.Api.warn('received erroneous response while getting games');
      return response;
    }

    return response;
  };

  static getJoinMeta = async (gameId: number): FetchResponse<GameMeta> => {
    Logger.Api.log(`fetching game ${gameId} meta`);

    const response = await WrappedFetch<GameMeta>(Backend.getDynamicUrl(Endpoint.Game, { id: gameId.toString(10) }), {
      method: 'GET',
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        id: 1,
        isPublic: true,
        name: 'DEV',
        playerCount: -1
      },
      requestOptions: {
        withCredentials: true
      }
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while getting game info');
      return response;
    }

    return response;
  };

  static joinGame = async (gameId: number, role: ZRPRole, password: string): FetchResponse<GameJoinResponse> => {
    Logger.Api.log(`send join game ${gameId} request as ${role}`);

    const response = await WrappedFetch<{ gameId: number; isRunning: boolean; role: ZRPRole; ownId: number }>(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        gameId: gameId,
        isRunning: false,
        role: role,
        ownId: 0
      },
      requestOptions: {
        withCredentials: true
      },
      body: JSON.stringify({
        gameId: gameId,
        password: password,
        role: role
      })
    });

    if (response.isError) {
      Logger.Api.warn('received erroneous response while joining game');
      return response;
    }

    return response;
  };
}
