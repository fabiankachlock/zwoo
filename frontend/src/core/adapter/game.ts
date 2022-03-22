import { defineStore } from 'pinia';
import { Backend, Endpoint } from '../services/api/apiConfig';
import { GameManagementService } from '../services/api/GameManagement';
import { GameNameValidator } from '../services/validator/gameName';
import { ZRPWebsocketAdapter } from '../services/ws/MessageDistributer';
import { ZRPMessageBuilder } from '../services/zrp/zrpBuilder';
import { ZRPOPCode, ZRPPayload, ZRPRole } from '../services/zrp/zrpTypes';
import { useGameEvents } from './play/events';

export const useGameConfig = defineStore('game-config', {
  state: () => ({
    gameId: '',
    name: '',
    host: false, // TODO: store role
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
        gameId: status.id,
        name: name
      });
      this.connect();
    },
    async join(id: number, password: string, asPlayer: boolean, asSpectator: boolean) {
      if (asPlayer && asSpectator) {
        throw new Error('cant join as player & spectator');
      }

      const status = await GameManagementService.joinGame(id, asPlayer ? ZRPRole.Player : ZRPRole.Spectator, password);

      this.$patch({
        inActiveGame: true,
        host: false,
        gameId: status.id,
        name: 'unknown yet?' // TODO: fill in name
      });
      this.connect();
    },
    async connect() {
      this._connection = new ZRPWebsocketAdapter(Backend.getUrl(Endpoint.Websocket) + `${this.gameId}`, this.gameId);
      const events = useGameEvents();
      this._connection.readMessages(events.handleIncomingEvent);
    },
    async sendEvent<C extends ZRPOPCode>(code: C, payload: ZRPPayload<C>) {
      if (this._connection) {
        this._connection.writeMessage(ZRPMessageBuilder.build(code, payload));
      }
    }
  }
});
