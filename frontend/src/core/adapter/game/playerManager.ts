import { defineStore } from 'pinia';

import { ZRPPlayerState, ZRPRole } from '@/core/domain/zrp/zrpTypes';

export type InGamePlayer = {
  id: number;
  username: string;
  role: ZRPRole;
  state: ZRPPlayerState;
};

export const usePlayerManager = defineStore('game-players', {
  state: () => ({
    _allPlayers: {} as Record<string, InGamePlayer>
  }),
  getters: {
    spectators: state => Object.values(state._allPlayers).filter(p => p.role === ZRPRole.Spectator),
    players: state => Object.values(state._allPlayers).filter(p => p.role !== ZRPRole.Spectator),
    bots: state => Object.values(state._allPlayers).filter(p => p.role === ZRPRole.Bot),
    host: state => Object.values(state._allPlayers).filter(p => p.role !== ZRPRole.Host)[0],
    allPlayers: state => Object.values(state._allPlayers),
    getPlayer: state => (playerId: number) => state._allPlayers[playerId],
    isPlayerActive: state => (playerId: number) => state._allPlayers[playerId]?.state === 'connected',
    getPlayerRole: state => (playerId: number) => state._allPlayers[playerId]?.role,
    getPlayerName: state => (playerId: number) => state._allPlayers[playerId]?.username
  },
  actions: {
    addPlayer(player: Omit<InGamePlayer, 'state'>) {
      this._allPlayers[player.id] = {
        id: player.id,
        role: player.role,
        username: player.username,
        state: 'connected' // default state is connected, since only connected players are allowed to go ingame
      };
    },
    removePlayer(playerId: number) {
      delete this._allPlayers[playerId];
    },
    setPlayerDisconnected(playerId: number) {
      const player = this._allPlayers[playerId];
      if (player) {
        player.state = 'disconnected';
      }
    },
    setPlayerConnected(playerId: number) {
      const player = this._allPlayers[playerId];
      if (player) {
        player.state = 'connected';
      }
    },
    reset() {
      this._allPlayers = {};
    }
  }
});
