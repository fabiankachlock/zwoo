import { CacheTTL, useDebounce } from '@/core/adapter/helper/useDebounce';

import { Leaderboard } from '../../api/entities/Game';
import { useApi } from '../helper/useApi';

export type LeaderBoardEntry = {
  position: number;
  name: string;
  wins: number;
};

const leaderBoardStore = {
  getLeaderBoard: useDebounce<LeaderBoardEntry[]>(async () => {
    const response = await useApi().loadLeaderBoard();
    if (response.isError) {
      return [];
    }
    const leaderboard = response.data.leaderboard;
    const groups: Record<number, Leaderboard['leaderboard']> = {};

    for (let i = 0; i < leaderboard.length; i++) {
      if (groups[leaderboard[i].wins]) {
        groups[leaderboard[i].wins].push(leaderboard[i]);
      } else {
        groups[leaderboard[i].wins] = [leaderboard[i]];
      }
    }

    return Object.keys(groups)
      .sort((a, b) => parseInt(b) - parseInt(a))
      .reduce((arr, key) => {
        return [
          ...arr,
          ...groups[key as unknown as number].map(player => ({
            name: player.username,
            wins: player.wins,
            position: arr.length + groups[key as unknown as number].length
          }))
        ];
      }, [] as LeaderBoardEntry[]);
  }, CacheTTL.minute * 5),

  getOwnPosition: useDebounce<number>(async () => {
    const response = await useApi().loadOwnLeaderBoardPosition();
    if (response.isError) {
      return -1;
    }
    return response.data.position;
  }, CacheTTL.minute * 5)
};

export const useLeaderBoard = () => leaderBoardStore;
