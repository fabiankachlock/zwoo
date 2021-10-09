export type GameStatusResponse = {
  id: string;
};

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameStatusResponse> => {
    // make api call
    console.log('create game:', { name, isPublic, password });

    return {
      id: 'some-test-id'
    };
  };
}
