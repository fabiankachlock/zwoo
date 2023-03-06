import { GameAdapter } from '../GameAdapter';
import { Backend, Endpoint } from '../restapi/ApiConfig';
import { GameWebsocket } from './Websocket';

export const WsGameAdapter: GameAdapter = {
  createConnection(gameId) {
    const url = Backend.getDynamicUrl(Endpoint.Websocket, { id: gameId });
    return new GameWebsocket(url);
  }
};
