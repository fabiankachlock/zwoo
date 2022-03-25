import { Backend, Endpoint } from './apiConfig';

export type LeaderBoardResponse = {
  leaderboard: {
    username: string;
    wins: number;
  }[];
};

export class LeaderBoardService {
  static async fetchLeaderBoards(): Promise<LeaderBoardResponse | undefined> {
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      return Promise.resolve({
        leaderboard: new Array(50).fill(null).map((_, index) => ({
          username: `user-${index}`,
          wins: 50 - index
        }))
      });
    }

    const req = await fetch(Backend.getUrl(Endpoint.LeaderBoard));

    if (req.status > 300) {
      return undefined;
    }

    const response = (await req.json()) as LeaderBoardResponse;
    // sort player to make sure
    response.leaderboard = response.leaderboard.sort((a, b) => a.wins - b.wins);
    return response;
  }
}
