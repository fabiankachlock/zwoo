export type GameStatusResponse = {
  id: string;
};

export class GameManagementService {
  static createGame = async (name: string, isPublic: boolean, password: string): Promise<GameStatusResponse> => {
    return {
      id: 'some-test-id'
    };
  };
}
