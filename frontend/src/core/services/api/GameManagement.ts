import { ZRPRole } from '../zrp/zrpTypes';
import { Backend, Endpoint } from './apiConfig';

export type GameStatusResponse = {
  id: number;
  name: string;
};

export type GameMeta = {
  id: number;
  name: string;
  isPublic: boolean;
  playerCount: number;
};

export type GameJoinMeta = {
  name: string;
  needsValidation: boolean;
};

export type GamesList = GameMeta[];

const _DummyGames: GamesList = [
  {
    name: 'Dev-Game',
    isPublic: true,
    id: 1,
    playerCount: 999
  },
  {
    name: 'Test-Public',
    isPublic: true,
    id: 2,
    playerCount: 1
  },
  {
    name: 'Test-Private',
    isPublic: false,
    id: 2,
    playerCount: 5
  }
];

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameStatusResponse> => {
    console.log('create game:', { name, isPublic, password });
    const req = await fetch(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      credentials: 'include',
      body: JSON.stringify({
        name: name,
        password: password,
        use_password: !isPublic,
        opcode: 1 // join as host / create game
      })
    });
    const result = await req.json();
    console.log(result);
    return {
      id: result.guid,
      name: name
    };
  };

  static listAll = async (): Promise<GamesList> => {
    // make api call
    console.log('load games');
    return _DummyGames;
  };

  static getJoinMeta = async (gameId: number): Promise<GameJoinMeta> => {
    return new Promise((res, rej) =>
      setTimeout(() => {
        const game = _DummyGames.find(g => g.id === gameId);
        if (game) {
          res({
            needsValidation: !game.isPublic,
            name: game.name
          });
        } else {
          rej({ message: 'not-found' });
        }
      }, 3000)
    );
  };

  static joinGame = async (gameId: number, role: ZRPRole, password: string): Promise<GameStatusResponse> => {
    console.log('join game', { gameId, password, role });

    const req = await fetch(Backend.getUrl(Endpoint.JoinGame), {
      method: 'POST',
      credentials: 'include',
      body: JSON.stringify({
        guid: gameId,
        password: password,
        opcode: role // join as host / create game
      })
    });

    const result = await req.json();
    console.log(result);
    return {
      id: result.guid,
      name: _DummyGames.find(g => g.id === gameId)?.name ?? 'no-game-name'
    };
  };
}
