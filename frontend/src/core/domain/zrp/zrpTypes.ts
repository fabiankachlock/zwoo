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

export const ZRP_VERSION = '4.3.0';

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
  GetAllGameProfiles = 204, // sender(host)
  AllGameProfiles = 205, // receiver
  SaveToProfile = 206, // send(host)
  UpdateGameProfile = 207, // send(host)
  ApplyGameProfile = 208, // send(host)
  DeleteGameProfile = 209, // send(host)
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
  BotNameExistsError = 425, // receiver
  PileEmptyError = 426, // receiver
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
  _System = -1,
  Host = 1,
  Player = 2,
  Spectator = 3,
  Bot = 4
}

export enum ZRPDecisionType {
  ColorPicker = 1,
  PlayerSelector = 2
}

export enum ZRPFeedbackKind {
  Individual = 1,
  Interaction = 2,
  Unaffected = 3
}

export enum ZRPFeedbackType {
  Skipped = 1,
  DirectionChanged = 2,
  PlayerHasDrawn = 3,
  MissedLast = 4,
  DeckSwapped = 5,
  ColorChanged = 6
}

export type ZRPPlayerState = 'disconnected' | 'connected';

export enum SettingsType {
  Readonly = 0,
  Numeric = 1,
  Boolean = 2
}

export enum GameProfileGroup {
  System = 1,
  User = 2
}

export type ZRPPayloadMap = {
  // General
  [ZRPOPCode.PlayerJoined]: ZRPPlayerJoinedPayload;
  [ZRPOPCode.SpectatorJoined]: ZRPNamePayload;
  [ZRPOPCode.PlayerLeft]: ZRPIdPayload;
  [ZRPOPCode.SpectatorLeft]: ZRPIdPayload;
  [ZRPOPCode.LeaveGame]: Record<string, never>;
  // Chat
  [ZRPOPCode.SendMessage]: ZRPSendChatMessagePayload;
  [ZRPOPCode.ReceiveMessage]: ZRPChatMessagePayload;
  // all players
  [ZRPOPCode.GetAllPlayers]: Record<string, never>;
  [ZRPOPCode.ListAllPlayers]: ZRPAllLobbyPlayersPayload;
  // Roles
  [ZRPOPCode.SpectatorToPlayer]: Record<string, never>;
  [ZRPOPCode.PlayerToSpectator]: ZRPIdPayload;
  [ZRPOPCode.PromotePlayerToHost]: ZRPIdPayload;
  [ZRPOPCode.PromotedToHost]: Record<string, never>;
  [ZRPOPCode.NewHost]: ZRPIdPayload;
  [ZRPOPCode.KickPlayer]: ZRPIdPayload;
  [ZRPOPCode.PlayerChangedRole]: ZRPPlayerChangedRolePayload;
  [ZRPOPCode.PlayerDisconnected]: ZRPIdPayload;
  [ZRPOPCode.PlayerReconnected]: ZRPIdPayload;
  // Keep alive
  [ZRPOPCode.KeepAlive]: Record<string, never>;
  [ZRPOPCode.AckKeepAlive]: Record<string, never>;
  // Lobby
  [ZRPOPCode.UpdateSetting]: ZRPSettingsChangePayload;
  [ZRPOPCode.SettingChanged]: ZRPSettingsChangePayload;
  [ZRPOPCode.GetAllSettings]: Record<string, never>;
  [ZRPOPCode.AllSettings]: ZRPSettingsPayload;
  [ZRPOPCode.GetAllGameProfiles]: Record<string, never>;
  [ZRPOPCode.AllGameProfiles]: ZRPGameProfilesPayload;
  [ZRPOPCode.SaveToProfile]: ZRPSaveToProfilePayload;
  [ZRPOPCode.UpdateGameProfile]: ZRPGameProfileIdPayload;
  [ZRPOPCode.ApplyGameProfile]: ZRPGameProfileIdPayload;
  [ZRPOPCode.DeleteGameProfile]: ZRPGameProfileIdPayload;
  [ZRPOPCode.AllSettings]: ZRPSettingsPayload;
  [ZRPOPCode.StartGame]: Record<string, never>;
  // Bots
  [ZRPOPCode.CreateBot]: ZRPCreateBotPayload;
  [ZRPOPCode.BotJoined]: Omit<ZRPPlayerJoinedPayload, 'isBot'>;
  [ZRPOPCode.BotLeft]: ZRPIdPayload;
  [ZRPOPCode.UpdateBot]: ZRPUpdateBotPayload;
  [ZRPOPCode.DeleteBot]: ZRPIdPayload;
  [ZRPOPCode.GetBots]: Record<string, never>;
  [ZRPOPCode.ListBots]: ZRPListBotsPayload;
  // Game
  [ZRPOPCode.GameStarted]: ZRPGameStartedPayload;
  [ZRPOPCode.StartTurn]: Record<string, never>;
  [ZRPOPCode.EndTurn]: Record<string, never>;
  [ZRPOPCode.RequestEndTurn]: Record<string, never>;
  [ZRPOPCode.PlaceCard]: ZRPCardPayload;
  [ZRPOPCode.DrawCard]: Record<string, never>;
  [ZRPOPCode.GetCards]: ZRPCardListPayload;
  [ZRPOPCode.RemoveCard]: ZRPCardListPayload;
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
  [ZRPOPCode.BotNameExistsError]: ZRPErrorPayload;
  [ZRPOPCode.PileEmptyError]: ZRPErrorPayload;
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

export type ZRPIdPayload = {
  id: number;
};

export type ZRPNamePayload = ZRPIdPayload & {
  username: string;
};

export type ZRPPlayerJoinedPayload = ZRPIdPayload &
  ZRPNamePayload & {
    wins: number;
    isBot: boolean;
  };

export type ZRPPlayerChangedRolePayload = ZRPIdPayload & {
  role: ZRPRole;
  wins: number;
};

export type ZRPSendChatMessagePayload = {
  message: string;
};

export type ZRPChatMessagePayload = ZRPSendChatMessagePayload & ZRPIdPayload;

export type ZRPAllLobbyPlayersPayload = {
  players: {
    id: number;
    username: string;
    role: ZRPRole;
    state: ZRPPlayerState;
    wins: number;
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
    title: Record<string, string>;
    description: Record<string, string>;
    type: SettingsType;
    isReadonly: boolean;
    min?: number;
    max?: number;
  }[];
};

export type ZRPGameProfilesPayload = {
  profiles: {
    id: string;
    name: string;
    group: GameProfileGroup;
  }[];
};

export type ZRPSaveToProfilePayload = {
  name: string;
};

export type ZRPGameProfileIdPayload = {
  id: string;
};

export type ZRPBotConfig = {
  type: number;
};

export type ZRPCreateBotPayload = {
  username: string;
  config: ZRPBotConfig;
};

export type ZRPUpdateBotPayload = {
  id: number;
  config: ZRPBotConfig;
};

export type ZRPListBotsPayload = {
  bots: {
    id: number;
    username: string;
    config: ZRPBotConfig;
    wins: number;
  }[];
};

export type ZRPGameStartedPayload = ZRPDeckPayload &
  ZRPPlayerCardAmountPayload & {
    pile: ZRPCardPayload;
  };

export type ZRPCardPayload = {
  type: number;
  symbol: number;
};

export type ZRPCardListPayload = {
  cards: ZRPCardPayload[];
};

export type ZRPFeedback = {
  type: ZRPFeedbackType;
  kind: ZRPFeedbackKind;
  args: Record<string, number>;
};

export type ZRPStateUpdatePayload = {
  pileTop: {
    type: number;
    symbol: number;
  };
  activePlayer: number;
  cardAmounts: Record<number, number>;
  feedback: ZRPFeedback[];
  currentDrawAmount?: number;
};

export type ZRPDeckPayload = {
  hand: {
    type: number;
    symbol: number;
  }[];
};

export type ZRPPlayerCardAmountPayload = {
  players: {
    id: number;
    username: string;
    cards: number;
    order: number;
    isActivePlayer: boolean;
  }[];
};

export type ZRPDecisionRequestPayload = {
  type: ZRPDecisionType;
  options: string[];
};

export type ZRPDecisionResponsePayload = {
  type: ZRPDecisionType;
  decision: number;
};

export type ZRPGameWinnerPayload = {
  id: number;
  summary: {
    id: number;
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
