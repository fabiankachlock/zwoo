import Logger from '../logging/logImport';
import { ZRPRole } from '../zrp/zrpTypes';
import { Backend, Endpoint } from './ApiConfig';
import { BackendErrorAble, parseBackendError } from './Errors';

export type GameJoinResponse = BackendErrorAble<{
  id: number;
  isRunning: boolean;
  role: ZRPRole;
}>;

export type GameMeta = {
  id: number;
  name: string;
  isPublic: boolean;
  playerCount: number;
};

export type GamesList = GameMeta[];

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameJoinResponse> => {
    Logger.Api.log(`creating ${isPublic ? 'public' : 'non-public'} game ${name}`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking create game response');
      return {
        id: 1,
        isRunning: false,
        role: ZRPRole.Host
      };
    }

    const req = await fetch(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        name: name,
        password: password,
        use_password: !isPublic,
        opcode: 1 // join as host / create game
      })
    });

    if (req.status !== 200) {
      Logger.Api.warn('received erroneous response while creating game');
      return {
        error: parseBackendError(await req.text())
      };
    }

    const result = (await req.json()) as { guid: number };

    return {
      id: result.guid,
      isRunning: false,
      role: ZRPRole.Host
    };
  };

  static listAll = async (): Promise<BackendErrorAble<GamesList>> => {
    // make api call
    Logger.Api.log('fetching all games');
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking get games response');
      return [];
    }

    const req = await fetch(Backend.getUrl(Endpoint.Games), {
      method: 'GET',
      credentials: 'include'
    });

    if (req.status !== 200) {
      Logger.Api.warn('received erroneous response while getting games');
      return {
        error: parseBackendError(await req.text())
      };
    }
    const result = (await req.json()) as { games: GamesList };
    return result.games;
  };

  static getJoinMeta = async (gameId: number): Promise<BackendErrorAble<GameMeta>> => {
    Logger.Api.log(`fetching game ${gameId} meta`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking get games response');
      return {
        id: 1,
        name: 'DEV',
        isPublic: true,
        playerCount: -1
      };
    }

    const req = await fetch(Backend.getDynamicUrl(Endpoint.Game, { id: gameId.toString(10) }), {
      method: 'GET',
      credentials: 'include'
    });

    if (req.status !== 200) {
      Logger.Api.warn('received erroneous response while getting game info');
      return {
        error: parseBackendError(await req.text())
      };
    }
    const result = (await req.json()) as GameMeta;
    return result;
  };

  static joinGame = async (gameId: number, role: ZRPRole, password: string): Promise<GameJoinResponse> => {
    Logger.Api.log(`send join game ${gameId} request as ${role}`);
    if (import.meta.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking join game response');
      return {
        id: gameId,
        isRunning: false,
        role: role
      };
    }

    const req = await fetch(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        guid: gameId,
        password: password,
        opcode: role // join as host / create game
      })
    });

    if (req.status !== 200) {
      Logger.Api.warn('received erroneous response while joining game');
      return {
        error: parseBackendError(await req.text())
      };
    }

    const result = (await req.json()) as { guid: number; isRunning: boolean; role: ZRPRole };
    return {
      id: result.guid,
      isRunning: result.isRunning,
      role: result.role
    };
  };
}
