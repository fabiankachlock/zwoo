import { CacheTTL, useDebounce } from '@/core/adapter/helper/useDebounce';
import { LeaderBoardService } from '@/core/api/restapi/LeaderBoard';

import { LeaderBoardPositionResponse, LeaderBoardResponse } from '../api/entities/Leaderboard';

export type LeaderBoardEntry = {
  position: number;
  name: string;
  wins: number;
};

const leaderBoardStore = {
  getLeaderBoard: useDebounce<LeaderBoardEntry[]>(async () => {
    const response = await LeaderBoardService.fetchLeaderBoard();
    if ('error' in response) {
      return [];
    }
    const leaderboard = (response as LeaderBoardResponse).leaderboard;
    const groups: Record<number, LeaderBoardResponse['leaderboard']> = {};

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
    const response = await LeaderBoardService.fetchOwnLeaderBoardPosition();
    if ('error' in response) {
      return -1;
    }
    return (response as LeaderBoardPositionResponse).position;
  }, CacheTTL.minute * 5)
};

export const useLeaderBoard = () => leaderBoardStore;
