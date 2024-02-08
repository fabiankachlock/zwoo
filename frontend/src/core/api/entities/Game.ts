import { ZRPRole } from '@/core/domain/zrp/zrpTypes';

export type GameJoinResponse = {
  gameId: number;
  isRunning: boolean;
  role: ZRPRole;
  ownId: number;
};

export type Leaderboard = {
  leaderboard: {
    username: string;
    wins: number;
  }[];
};

export type LeaderboardPosition = {
  position: number;
};

export type GameMeta = {
  id: number;
  name: string;
  isPublic: boolean;
  playerCount: number;
};

export type GamesList = {
  games: GameMeta[];
};

export type CreateGame = {
  name: string;
  password: string;
  usePassword: boolean;
};

export type JoinGame = {
  gameId: number;
  password: string;
  role: number;
};
