export type GameStatusResponse = {
  id: string;
};

export type GameMeta = {
  id: string;
  name: string;
  isPublic: boolean;
  playerCount: number;
};

export type GamesList = GameMeta[];

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameStatusResponse> => {
    // make api call
    console.log('create game:', { name, isPublic, password });

    return {
      id: 'some-test-id'
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
}
