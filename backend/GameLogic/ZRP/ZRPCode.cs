namespace ZwooGameLogic.ZRP;

public enum ZRPCode
{
    // General
    // - players
    /// <summary>send</summary>
    PlayerJoined = 100,
    /// <summary>send</summary>
    SpectatorJoined = 101,
    /// <summary>send</summary>
    PlayerLeft = 102,
    /// <summary>send</summary>
    SpectatorLeft = 103,
    /// <summary>receive</summary>
    PlayerLeaves = 106,
    // - chat
    /// <summary>receive</summary>
    CreateChatMessage = 104,
    /// <summary>send</summary>
    SendChatMessage = 105,
    // - all players
    /// <summary>receive</summary>
    GetLobby = 108,
    /// <summary>send</summary>
    SendLobby = 109,
    // - player role
    ///<summary>receive</summary>
    SpectatorToPlayer = 110,
    ///<summary>receive</summary>
    PlayerToSpectator = 111,
    ///<summary>receive</summary>
    PlayerToHost = 112,
    ///<summary>send</summary>
    PromotedToHost = 113,
    ///<summary>send</summary>
    HostChanged = 114,
    ///<summary>receive</summary>
    KickPlayer = 115,
    ///<summary>send</summary>
    PlayerChangedRole = 116,
    ///<summary>send</summary>
    PlayerDisconnected = 117,
    ///<summary>send</summary>
    PlayerReconnected = 118,
    ///<summary>receive</summary>
    KeepAlive = 198,
    ///<summary>send</summary>
    AckKeepAlive = 199,
    // Lobby
    /// <summary>receive</summary>
    UpdateSetting = 200,
    /// <summary>send</summary>
    SettingChanged = 201,
    /// <summary>receive</summary>
    GetAllSettings = 202,
    /// <summary>send</summary>
    SendAllSettings = 203,
    /// <summary>receive</summary>
    StartGame = 210,
    /// <summary>receive</summary>
    CreateBot = 230,
    /// <summary>send</summary>
    BotJoined = 231,
    /// <summary>send</summary>
    BotLeft = 232,
    /// <summary>receive</summary>
    UpdateBot = 233,
    /// <summary>receive</summary>
    DeleteBot = 235,
    /// <summary>receive</summary>
    GetBots = 236,
    /// <summary>send</summary>
    SendBots = 237,
    // Game
    /// <summary>send</summary>
    GameStarted = 300,
    /// <summary>send</summary>
    StartTurn = 301,
    /// <summary>send</summary>
    EndTurn = 302,
    /// <summary>receive</summary>
    RequestEndTurn = 303,
    /// <summary>receive</summary>
    PlaceCard = 304,
    /// <summary>receive</summary>
    DrawCard = 305,
    /// <summary>send</summary>
    SendCard = 306,
    /// <summary>send</summary>
    RemoveCard = 307,
    /// <summary>send</summary>
    StateUpdated = 308,
    /// <summary>receive</summary>
    GetHand = 310,
    /// <summary>send</summary>
    SendHand = 311,
    /// <summary>receive</summary>
    GetCardAmount = 312,
    /// <summary>send</summary>
    SendCardAmount = 313,
    /// <summary>receive</summary>
    GetPileTop = 314,
    /// <summary>send</summary>
    SendPileTop = 315,
    /// <summary>send</summary>
    GetPlayerDecision = 316,
    /// <summary>receive</summary>
    ReceiveDecision = 317,
    /// <summary>send</summary>
    PlayerWon = 399,
    // Errors
    GeneralError = 400, // send
    MessageToLongError = 401, // send
    AccessDeniedError = 420, // send
    LobbyFullError = 421, // send
    EndTurnError = 433, // send
    PlaceCardError = 434, // send
}
