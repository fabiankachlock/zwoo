export type LeaderBoardResponse = {
  leaderboard: {
    username: string;
    wins: number;
  }[];
};

export type LeaderBoardPositionResponse = {
  position: number;
};
