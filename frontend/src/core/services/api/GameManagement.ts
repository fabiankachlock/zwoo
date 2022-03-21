import { Backend, Endpoint } from './apiConfig';

export type GameStatusResponse = {
  id: string;
};

export type GameMeta = {
  id: string;
  name: string;
  isPublic: boolean;
  playerCount: number;
};

export type GameJoinMeta = {
  name: string;
  needsValidation: boolean;
};

export type GamesList = GameMeta[];

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameStatusResponse> => {
    // make api call
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
      id: result.guid
    };
  };

  static listAll = async (): Promise<GamesList> => {
    // make api call
    console.log('load games');
    return [
      {
        name: 'Test-Public',
        isPublic: true,
        id: 'g-pub',
        playerCount: 1
      },
      {
        name: 'Test-Private',
        isPublic: false,
        id: 'g-prv',
        playerCount: 5
      }
    ];
  };

  static getJoinMeta = async (gameId: string): Promise<GameJoinMeta> => {
    return new Promise(res =>
      setTimeout(() => {
        res({
          needsValidation: true,
          name: `Some-Game (${gameId})`
        });
      }, 3000)
    );
  };

  static joinGame = async (gameId: string, password: string): Promise<void> => {
    console.log('join game', gameId, password);
  };
}
