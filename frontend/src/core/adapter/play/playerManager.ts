import { defineStore } from 'pinia';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';
import { uniqueBy } from '@/core/services/utils';

export type InGmePlayer = {
  id: string;
  username: string;
  role: ZRPRole;
  state: 'disconnected' | 'connected';
};

export const usePlayerManager = defineStore('game-players', {
  state: () => ({
    _allPlayers: [] as InGmePlayer[]
  }),
  getters: {
    spectators: state => state._allPlayers.filter(p => p.role === ZRPRole.Spectator),
    players: state => state._allPlayers.filter(p => p.role !== ZRPRole.Spectator),
    host: state => state._allPlayers.filter(p => p.role !== ZRPRole.Host)[0],
    allPlayers: state => state._allPlayers,
    getPlayer: state => (playerId: string) => state._allPlayers.find(p => p.id === playerId)
  },
  actions: {
    addPlayer(player: Omit<InGmePlayer, 'state'>) {
      this._allPlayers = uniqueBy(
        [
          ...this._allPlayers,
          {
            id: player.id,
            role: player.role,
            username: player.username,
            state: 'disconnected'
          }
        ],
        p => p.id
      );
    },
    removePlayer(playerId: string) {
      this._allPlayers = this._allPlayers.filter(p => p.id !== playerId);
    },
    setPlayerDisconnected(playerId: string) {
      const player = this._allPlayers.find(p => p.id === playerId);
      if (player) {
        player.state = 'disconnected';
      }
    },
    setPlayerConnected(playerId: string) {
      const player = this._allPlayers.find(p => p.id === playerId);
      if (player) {
        player.state = 'connected';
      }
    },
    reset() {
      this._allPlayers = [];
    }
  }
});
