import { defineStore } from 'pinia';
import { Backend, Endpoint } from '../services/api/apiConfig';
import { GameManagementService } from '../services/api/GameManagement';
import { GameNameValidator } from '../services/validator/gameName';
import { ZRPWebsocketAdapter } from '../services/ws/MessageDistributer';
import { ZRPOPCode, ZRPPayload } from '../services/zrp/zrpTypes';
import { useGameEvents } from './play/events';

export const useGameConfig = defineStore('game-config', {
  state: () => ({
    gameId: '',
    host: false,
    inActiveGame: false,
    _connection: undefined as ZRPWebsocketAdapter | undefined
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
      this.connect();
    },
    async join(gameId: string) {
      console.log('join', gameId);
    },
    async connect() {
      this._connection = new ZRPWebsocketAdapter(Backend.getUrl(Endpoint.Websocket), this.gameId);
      const events = useGameEvents();
      this._connection.readMessages(events.handleIncomingEvent);
    },
    async sendEvent<C extends ZRPOPCode>(code: C, payload: ZRPPayload<C>) {
      if (this._connection) {
        this._connection.writeMessage({
          code: code,
          data: payload
        });
      }
    }
  }
});
