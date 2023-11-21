namespace Zwoo.GameEngine.Lobby;

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
