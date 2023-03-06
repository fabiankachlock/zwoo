import { RealtimeGameMessageAdapter } from '../domain/zrp/zrpInterfaces';

export interface GameAdapter {
  createConnection(gameId: string): RealtimeGameMessageAdapter;
}
