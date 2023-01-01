namespace ZwooGameLogic.Lobby;

public enum LobbyResult: int
{
    Success,
    ErrorWrongPassword,
    ErrorLobbyFull,
    ErrorAlredyInitialized,
    ErrorAlredyInGame,
    ErrorInvalidPlayer,
    Error
}
