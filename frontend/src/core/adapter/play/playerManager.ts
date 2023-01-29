import { defineStore } from 'pinia';

import { ZRPPlayerState, ZRPRole } from '@/core/services/zrp/zrpTypes';

export type InGamePlayer = {
  id: string;
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
    getPlayer: state => (playerId: string) => state._allPlayers[playerId],
    isPlayerActive: state => (playerId: string) => state._allPlayers[playerId]?.state === 'connected',
    getPlayerRole: state => (playerId: string) => state._allPlayers[playerId]?.role
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
    removePlayer(playerId: string) {
      delete this._allPlayers[playerId];
    },
    setPlayerDisconnected(playerId: string) {
      const player = this._allPlayers[playerId];
      if (player) {
        player.state = 'disconnected';
      }
    },
    setPlayerConnected(playerId: string) {
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
