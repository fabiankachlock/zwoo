import { GameAdapter } from '../GameAdapter';
import { Backend, Endpoint } from '../restapi/ApiConfig';
import { GameWebsocket } from './Websocket';

export const WsGameAdapter = (apiUrl: string, wsOverride: string): GameAdapter => ({
  createConnection(gameId) {
    const url = Backend.from(apiUrl, wsOverride).getDynamicUrl(Endpoint.Websocket, { id: gameId });
    return new GameWebsocket(url);
  }
});
