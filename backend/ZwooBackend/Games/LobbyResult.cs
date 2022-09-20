namespace ZwooBackend.Games;

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
