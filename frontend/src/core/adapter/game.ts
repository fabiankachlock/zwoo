import { defineStore } from 'pinia';
import { GameManagementService } from '../services/api/GameManagement';
import { GameNameValidator } from '../services/validator/gameName';

export const useGameConfig = defineStore('game-config', {
  state: () => ({
    gameId: '',
    host: false,
    inActiveGame: false
  }),
  actions: {
    async create(name: string, isPublic: boolean, password: string) {
      const nameValid = new GameNameValidator().validate(name);
      if (!nameValid.isValid) throw nameValid.getErrors();

      const status = await GameManagementService.createGame(name, isPublic, password);

      this.$patch({
        inActiveGame: true,
        host: true,
        gameId: status.id
      });
    }
  }
});
