namespace ZwooGameLogic.Lobby;

public enum LobbyResult : int
{
    Success,
    ErrorWrongPassword,
    ErrorLobbyFull,
    ErrorAlreadyInitialized,
    ErrorAlreadyInGame,
    ErrorInvalidPlayer,
    Error
}
