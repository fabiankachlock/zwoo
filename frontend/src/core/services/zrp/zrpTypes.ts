export type ZRPMessage<T extends {} | ZRPOPCode = Record<string, unknown>> = T extends ZRPOPCode
  ? {
      code: T;
      data: ZRPPayloadMap[T];
    }
  : {
      code: ZRPOPCode;
      data: T;
    };

export type ZRPPayload<T extends ZRPOPCode> = ZRPPayloadMap[T];

export enum ZRPOPCode {
  // General
  // - players
  PlayerJoined = 100, // receiver
  SpectatorJoined = 101, // receiver
  PlayerLeft = 102, // receiver
  SpectatorLeft = 103, // receiver
  // - chat
  SendMessage = 104, // sender
  ReceiveMessage = 105, // receiver
  // - all players
  GetAllPlayers = 108, // sender
  ListAllPlayers = 109, // receiver
  // - player role
  SpectatorWantsToPlay = 110, // sender (spectator)
  PlayerWantsToSpectate = 111, // sender (player)
  PromotePlayerToHost = 112, // sender (host)
  PromoteToHost = 113, // receiver(host)
  NewHost = 114, // receiver(player/spectator)
  KickPlayer = 115, // sender (host)
  PlayerChangedRole = 116, // receiver
  // Lobby
  ChangeSettings = 200, // sender (host)
  SettingsUpdated = 201, // receiver
  StartGame = 202, // sender (host)
  // Game
  GameStarted = 300, // receiver
  StartTurn = 301, // receiver
  EndTurn = 302, // receiver
  RequestEndTurn = 303, // sender
  PlaceCard = 304, // sender
  DrawCard = 305, // sender
  GetCard = 306, // receiver
  RemoveCard = 307, // receiver
  NewCardOnPile = 308, // receiver
  PlayerWon = 399, // receiver
  // Errors
  GeneralError = 400, // receiver
  EndTurnError = 433, // receiver
  PlaceCardError = 434, // receiver
  // internal Errors
  _UnknownError = 900,
  _ConnectionError = 901,
  _ConnectionClosed = 911,
  _ClientError = 920,
  _DecodingError = 921,
  // internal messages
  _Connected = 930
}

export enum ZRPRole {
  Host = 1,
  Player = 2,
  Spectator = 3
}

export type ZRPPayloadMap = {
  // General
  [ZRPOPCode.PlayerJoined]: ZRPJoinedGamePayload;
  [ZRPOPCode.SpectatorJoined]: ZRPJoinedGamePayload;
  [ZRPOPCode.PlayerLeft]: ZRPLeftGamePayload;
  [ZRPOPCode.SpectatorLeft]: ZRPLeftGamePayload;
  // Chat
  [ZRPOPCode.SendMessage]: ZRPSendChatMessagePayload;
  [ZRPOPCode.ReceiveMessage]: ZRPChatMessagePayload;
  // all players
  [ZRPOPCode.GetAllPlayers]: Record<string, never>;
  [ZRPOPCode.ListAllPlayers]: ZRPAllLobbyPlayersPayload;
  // Roles
  [ZRPOPCode.SpectatorWantsToPlay]: Record<string, never>;
  [ZRPOPCode.PlayerWantsToSpectate]: ZRPNamePayload;
  [ZRPOPCode.PromotePlayerToHost]: ZRPNamePayload;
  [ZRPOPCode.PromoteToHost]: Record<string, never>;
  [ZRPOPCode.NewHost]: ZRPNamePayload;
  [ZRPOPCode.KickPlayer]: ZRPNamePayload;
  [ZRPOPCode.PlayerChangedRole]: ZRPPlayerWithRolePayload;
  // Lobby
  [ZRPOPCode.ChangeSettings]: ZRPSettingsChangePayload;
  [ZRPOPCode.SettingsUpdated]: ZRPSettingsChangePayload;
  [ZRPOPCode.StartGame]: Record<string, never>;
  // Game
  [ZRPOPCode.GameStarted]: Record<string, never>;
  [ZRPOPCode.StartTurn]: Record<string, never>;
  [ZRPOPCode.EndTurn]: Record<string, never>;
  [ZRPOPCode.RequestEndTurn]: Record<string, never>;
  [ZRPOPCode.PlaceCard]: ZRPCardPayload;
  [ZRPOPCode.DrawCard]: Record<string, never>;
  [ZRPOPCode.GetCard]: ZRPCardPayload;
  [ZRPOPCode.RemoveCard]: ZRPCardPayload;
  [ZRPOPCode.NewCardOnPile]: ZRPCardPayload;
  [ZRPOPCode.PlayerWon]: ZRPGameWinnerPayload;
  // Errors
  [ZRPOPCode.GeneralError]: ZRPErrorPayload;
  [ZRPOPCode.EndTurnError]: ZRPErrorPayload;
  [ZRPOPCode.PlaceCardError]: ZRPErrorPayload;
  // internal Errors
  [ZRPOPCode._UnknownError]: ZRPInternalErrorPayload;
  [ZRPOPCode._ConnectionError]: ZRPInternalErrorPayload;
  [ZRPOPCode._ConnectionClosed]: ZRPInternalErrorPayload;
  [ZRPOPCode._ClientError]: ZRPInternalErrorPayload;
  [ZRPOPCode._DecodingError]: ZRPInternalErrorPayload;
  // internal messages
  [ZRPOPCode._Connected]: Record<string, never>;
};

export type ZRPNamePayload = {
  username: string;
};

export type ZRPJoinedGamePayload = ZRPNamePayload & {
  wins: number;
};

export type ZRPLeftGamePayload = ZRPNamePayload;

export type ZRPPlayerWithRolePayload = ZRPNamePayload & {
  wins: number;
  role: ZRPRole;
};

export type ZRPSendChatMessagePayload = {
  message: string;
};

export type ZRPChatMessagePayload = ZRPSendChatMessagePayload & {
  username: string;
  role: ZRPRole;
};

export type ZRPAllLobbyPlayersPayload = {
  players: ZRPPlayerWithRolePayload[];
};

export type ZRPSettingsChangePayload = {
  setting: string;
  value: boolean | number;
};

export type ZRPCardPayload = {
  type: number;
  symbol: number;
};

export type ZRPGameWinnerPayload = {
  username: string;
  wins: number;
};

export type ZRPErrorPayload = {
  message: string;
};

export type ZRPInternalErrorPayload = {
  messageKey: string;
  thrownError: unknown | Error;
  messageSent?: ZRPMessage;
  rawMessageSent?: string;
  rawReceivedContent?: string;
  timestamp: number;
};
