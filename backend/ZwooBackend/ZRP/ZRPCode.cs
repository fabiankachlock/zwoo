namespace ZwooBackend.ZRP;

public enum ZRPCode
{
    // General
    // - players
    PlayerJoined = 100, // send
    SpectatorJoined = 101, // send
    PlayerLeft = 102, // send
    SpectatorLeft = 103, // send
    PlayerLeftGame = 106, // receive
    // - chat
    PushMessage = 104, // receive
    DistributeMessage = 105, // send
    // - all players
    GetAllPlayers = 108, // receive
    ListAllPlayers = 109, // send
    // - player role
    SpectatorWantsToPlay = 110, // receive
    PlayerWantsToSpectate = 111, // receive
    PromotePlayerToHost = 112, // receive
    PromotedToHost = 113, // send
    HostChanged = 114, // send
    KickPlayer = 115, // receive
    PlayerChangedRole = 116, // send

    KeepAlive = 198, // receive
    AckKeepAlive = 199, // send
    // Lobby
    UpdateSetting = 200, // receive
    ChangedSettings = 201, // send
    GetAllSettings = 202, // receive
    AllSettings = 203, // send
    StartGame = 210, // receive
    // Game
    GameStarted = 300, // send
    StartTurn = 301, // send
    EndTurn = 302, // send
    RequestEndTurn = 303, // receive
    PlaceCard = 304, // receive
    DrawCard = 305, // receive
    SendCard = 306, // send
    RemoveCard = 307, // send
    StateUpdated = 308, // send
    GetHand = 310, // receive
    SendHand = 311, // send
    GetCardAmount = 312, // receive
    SendCardAmount = 313, // send
    GetPileTop = 314, // receive
    SendPileTop = 315, // send
    GetPlayerDecision = 316, // send
    ReceiveDecision = 317, // receive
    PlayerWon = 399, // send
    // Errors
    GeneralError = 400, // send
    MessagetoLongError = 401, // send
    AccessDeniedError = 420, // send
    LobbyFullError = 421, // send
    EndTurnError = 433, // send
    PlaceCardError = 434, // send
}
