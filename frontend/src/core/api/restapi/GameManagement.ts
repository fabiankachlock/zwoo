import { AppConfig } from '@/config';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import Logger from '@/core/services/logging/logImport';

import { BackendErrorAble } from '../ApiError';
import { GameJoinResponse, GameMeta, GamesList } from '../entities/Game';
import { Backend, Endpoint } from './ApiConfig';
import { WrappedFetch } from './FetchWrapper';

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameJoinResponse> => {
    Logger.Api.log(`creating ${isPublic ? 'public' : 'non-public'} game ${name}`);
    if (!AppConfig.UseBackend) {
      Logger.Api.debug('mocking create game response');
    }

    const response = await WrappedFetch<{ guid: number }>(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      fallbackValue: { guid: 1 },
      requestOptions: {
        withCredentials: true
      },
      body: JSON.stringify({
        name: name,
        password: password,
        use_password: !isPublic,
        opcode: 1 // join as host / create game
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while creating game');
      return {
        error: response.error
      };
    }

    return {
      id: response.data!.guid,
      isRunning: false,
      role: ZRPRole.Host
    };
  };

  static listAll = async (): Promise<BackendErrorAble<GamesList>> => {
    Logger.Api.log('fetching all games');

    const response = await WrappedFetch<{ games: GamesList }>(Backend.getUrl(Endpoint.Games), {
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
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while getting games');
      return {
        error: response.error
      };
    }

    return response.data!.games;
  };

  static getJoinMeta = async (gameId: number): Promise<BackendErrorAble<GameMeta>> => {
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

    if (response.error) {
      Logger.Api.warn('received erroneous response while getting game info');
      return {
        error: response.error
      };
    }

    return response.data!;
  };

  static joinGame = async (gameId: number, role: ZRPRole, password: string): Promise<GameJoinResponse> => {
    Logger.Api.log(`send join game ${gameId} request as ${role}`);

    const response = await WrappedFetch<{ guid: number; isRunning: boolean; role: ZRPRole }>(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      useBackend: AppConfig.UseBackend,
      fallbackValue: {
        guid: gameId,
        isRunning: false,
        role: role
      },
      requestOptions: {
        withCredentials: true
      },
      body: JSON.stringify({
        guid: gameId,
        password: password,
        opcode: role // join as host / create game
      })
    });

    if (response.error) {
      Logger.Api.warn('received erroneous response while joining game');
      return {
        error: response.error
      };
    }

    return {
      id: response.data!.guid,
      isRunning: response.data!.isRunning,
      role: response.data!.role
    };
  };
}