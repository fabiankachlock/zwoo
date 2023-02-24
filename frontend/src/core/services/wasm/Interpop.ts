/**
 *  An interface for the provided api of the wasm adapter
 */
export type CSharpExport = {
  GameManager: {
    CreateGame: (name: string, isPublic: boolean) => void;
    CloseGame: () => void;
    SendEvent: (code: number, payload: unknown) => void;
  };
  LocalNotificationAdapter: {
    OnMessage: (callback: (code: number, payload: unknown) => void) => void;
    OnDisconnect: (callback: () => void) => void;
  };
};
