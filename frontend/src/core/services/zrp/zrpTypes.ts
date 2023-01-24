// eslint-disable-next-line @typescript-eslint/ban-types
export type ZRPMessage<T extends unknown | ZRPOPCode = Record<string, unknown>> = T extends ZRPOPCode
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
  LeaveGame = 106, // sender
  // - chat
  SendMessage = 104, // sender
  ReceiveMessage = 105, // receiver
  // - all players
  GetAllPlayers = 108, // sender
  ListAllPlayers = 109, // receiver
  // - player role
  SpectatorToPlayer = 110, // sender (spectator)
  PlayerToSpectator = 111, // sender (player)
  PromotePlayerToHost = 112, // sender (host)
  PromotedToHost = 113, // receiver(host)
  NewHost = 114, // receiver(player/spectator)
  KickPlayer = 115, // sender (host)
  PlayerChangedRole = 116, // receiver
  PlayerDisconnected = 117, // receiver
  PlayerReconnected = 118, // receiver

  KeepAlive = 198, // sender
  AckKeepAlive = 199, //  receiver
  // Lobby
  UpdateSetting = 200, // sender (host)
  SettingChanged = 201, // receiver
  GetAllSettings = 202, // sender
  AllSettings = 203, // receiver
  StartGame = 210, // sender (host)
  // Bots
  CreateBot = 230, // sender(host)
  BotJoined = 231, // receiver
  BotLeft = 232, // receiver
  UpdateBot = 233, // sender(host)
  DeleteBot = 235, // sender(host)
  GetBots = 236, // sender(host)
  ListBots = 237, // receiver
  // Game
  GameStarted = 300, // receiver
  StartTurn = 301, // receiver
  EndTurn = 302, // receiver
  RequestEndTurn = 303, // sender
  PlaceCard = 304, // sender
  DrawCard = 305, // sender
  GetCards = 306, // receiver
  RemoveCard = 307, // receiver
  StateUpdate = 308, // receiver
  RequestHand = 310, //sender
  GetHand = 311, //receiver
  RequestPlayerCardAmount = 312, // sender
  GetPlayerCardAmount = 313, // receiver
  RequestPileTop = 314, // sender
  GetPileTop = 315, // receiver
  GetPlayerDecision = 316, // receiver
  SendPlayerDecision = 317, // sender
  PlayerWon = 399, // receiver
  // Errors
  GeneralError = 400, // receiver
  MessageTooLongError = 401, // receiver
  AccessDeniedError = 420, // receiver
  LobbyFullError = 421, // receiver
  EndTurnError = 433, // receiver
  PlaceCardError = 434, // receiver
  // internal Errors
  _UnknownError = 900,
  _ConnectionError = 901,
  _ConnectionClosed = 911,
  _ClientError = 920,
  _DecodingError = 921,
  // internal messages
  _Connected = 930,
  _ResetState = 932
}

export enum ZRPRole {
  Host = 1,
  Player = 2,
  Spectator = 3,
  Bot = 4
}

export type ZRPPlayerState = 'disconnected' | 'connected';

export type ZRPPayloadMap = {
  // General
  [ZRPOPCode.PlayerJoined]: ZRPNamePayload;
  [ZRPOPCode.SpectatorJoined]: ZRPNamePayload;
  [ZRPOPCode.PlayerLeft]: ZRPNamePayload;
  [ZRPOPCode.SpectatorLeft]: ZRPNamePayload;
  [ZRPOPCode.LeaveGame]: Record<string, never>;
  // Chat
  [ZRPOPCode.SendMessage]: ZRPSendChatMessagePayload;
  [ZRPOPCode.ReceiveMessage]: ZRPChatMessagePayload;
  // all players
  [ZRPOPCode.GetAllPlayers]: Record<string, never>;
  [ZRPOPCode.ListAllPlayers]: ZRPAllLobbyPlayersPayload;
  // Roles
  [ZRPOPCode.SpectatorToPlayer]: Record<string, never>;
  [ZRPOPCode.PlayerToSpectator]: ZRPNamePayload;
  [ZRPOPCode.PromotePlayerToHost]: ZRPNamePayload;
  [ZRPOPCode.PromotedToHost]: Record<string, never>;
  [ZRPOPCode.NewHost]: ZRPNamePayload;
  [ZRPOPCode.KickPlayer]: ZRPNamePayload;
  [ZRPOPCode.PlayerChangedRole]: ZRPPlayerWithRolePayload;
  [ZRPOPCode.PlayerDisconnected]: ZRPNamePayload;
  [ZRPOPCode.PlayerReconnected]: ZRPNamePayload;
  // Keep alive
  [ZRPOPCode.KeepAlive]: Record<string, never>;
  [ZRPOPCode.AckKeepAlive]: Record<string, never>;
  // Lobby
  [ZRPOPCode.UpdateSetting]: ZRPSettingsChangePayload;
  [ZRPOPCode.SettingChanged]: ZRPSettingsChangePayload;
  [ZRPOPCode.GetAllSettings]: Record<string, never>;
  [ZRPOPCode.AllSettings]: ZRPSettingsPayload;
  [ZRPOPCode.StartGame]: Record<string, never>;
  // Bots
  [ZRPOPCode.CreateBot]: ZRPBotPayload;
  [ZRPOPCode.BotJoined]: ZRPNamePayload;
  [ZRPOPCode.BotLeft]: ZRPNamePayload;
  [ZRPOPCode.UpdateBot]: ZRPBotPayload;
  [ZRPOPCode.DeleteBot]: ZRPNamePayload;
  [ZRPOPCode.GetBots]: Record<string, never>;
  [ZRPOPCode.ListBots]: ZRPListBotsPayload;
  // Game
  [ZRPOPCode.GameStarted]: Record<string, never>;
  [ZRPOPCode.StartTurn]: Record<string, never>;
  [ZRPOPCode.EndTurn]: Record<string, never>;
  [ZRPOPCode.RequestEndTurn]: Record<string, never>;
  [ZRPOPCode.PlaceCard]: ZRPCardPayload;
  [ZRPOPCode.DrawCard]: Record<string, never>;
  [ZRPOPCode.GetCards]: ZRPCardListPayload;
  [ZRPOPCode.RemoveCard]: ZRPCardPayload;
  [ZRPOPCode.StateUpdate]: ZRPStateUpdatePayload;
  [ZRPOPCode.RequestHand]: Record<string, never>;
  [ZRPOPCode.GetHand]: ZRPDeckPayload;
  [ZRPOPCode.RequestPlayerCardAmount]: Record<string, never>;
  [ZRPOPCode.GetPlayerCardAmount]: ZRPPlayerCardAmountPayload;
  [ZRPOPCode.RequestPileTop]: Record<string, never>;
  [ZRPOPCode.GetPileTop]: ZRPCardPayload;
  [ZRPOPCode.GetPlayerDecision]: ZRPDecisionRequestPayload;
  [ZRPOPCode.SendPlayerDecision]: ZRPDecisionResponsePayload;
  [ZRPOPCode.PlayerWon]: ZRPGameWinnerPayload;
  // Errors
  [ZRPOPCode.GeneralError]: ZRPErrorPayload;
  [ZRPOPCode.MessageTooLongError]: ZRPErrorPayload;
  [ZRPOPCode.AccessDeniedError]: ZRPErrorPayload;
  [ZRPOPCode.LobbyFullError]: ZRPErrorPayload;
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
  [ZRPOPCode._ResetState]: Record<string, never>;
};

export type ZRPNamePayload = {
  username: string;
};

export type ZRPPlayerWithRolePayload = ZRPNamePayload & {
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
  players: {
    username: string;
    role: ZRPRole;
    state: ZRPPlayerState;
  }[];
};

export type ZRPSettingsChangePayload = {
  setting: string;
  value: number;
};

export type ZRPSettingsPayload = {
  settings: {
    setting: string;
    value: number;
  }[];
};

export type ZRPBotConfig = {
  type: number;
};

export type ZRPBotPayload = {
  username: string;
  config: ZRPBotConfig;
};

export type ZRPListBotsPayload = {
  bots: ZRPBotPayload[];
};

export type ZRPCardPayload = {
  type: number;
  symbol: number;
};

export type ZRPCardListPayload = {
  cards: ZRPCardPayload[];
};

export type ZRPStateUpdatePayload = {
  pileTop: {
    type: number;
    symbol: number;
  };
  activePlayer: string;
  activePlayerCardAmount: number;
  lastPlayer: string;
  lastPlayerCardAmount: number;
};

export type ZRPDeckPayload = {
  hand: {
    type: number;
    symbol: number;
  }[];
};

export type ZRPPlayerCardAmountPayload = {
  players: {
    username: string;
    cards: number;
    order: number;
    isActivePlayer: boolean;
  }[];
};

export type ZRPDecisionRequestPayload = {
  type: number;
};

export type ZRPDecisionResponsePayload = {
  type: number;
  decision: number;
};

export type ZRPGameWinnerPayload = {
  username: string;
  summary: {
    username: string;
    position: number;
    score: number;
  }[];
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
