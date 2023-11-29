using System.Net.WebSockets;

namespace Zwoo.Backend.Shared.Api.Game;

public interface IGameConnectionsService
{
    /// <summary>
    /// register a new active connection
    /// </summary>
    /// <param name="gameId">the id of the game</param> 
    /// <param name="playerId">the id of the player</param>
    /// <param name="ws">a websocket object</param>
    /// <returns></returns>
    public bool AddConnection(long gameId, long playerId, WebSocket ws, CancellationTokenSource tokenSource);

    /// <summary>
    /// remove an active websocket connection
    /// </summary>
    /// <param name="gameId">the id of the game</param>
    /// <param name="playerId">the id of the player</param>
    /// <returns>whether a connection was removed or not</returns>
    public bool RemoveConnection(long gameId, long playerId);

    /// <summary>
    /// returns if a player has already an active websocket connection
    /// </summary>
    /// <param name="playerId">the players id</param>
    /// <returns></returns>
    public bool HasConnection(long playerId);

    /// <summary>
    /// returns the amount of active connections for a game
    /// </summary>
    /// <param name="gameId">the id of the game</param>
    /// <returns>the amount if active connections</returns>
    public int ConnectionsCount(long gameId);

    /// <summary>
    /// handle a websocket connect
    /// </summary>
    /// <param name="gameId">the associated games id</param>
    /// <param name="userId">the associated users id</param>
    /// <param name="ws">the websocket object</param>
    /// <param name="token">the connection cancellation token</param>
    /// <param name="source">a task completion source waiting until the connection is closed</param>
    public void Handle(long gameId, long userId, WebSocket ws, CancellationToken token, TaskCompletionSource source);

    /// <summary>
    /// send a message to an individual player
    /// </summary>
    /// <param name="playerId">the players id</param>
    /// <param name="content">the content of the message</param>
    /// <param name="messageType">the message type</param>
    /// <param name="isEndOfMessage">bool indicating whether this is the end of a message</param>
    /// <returns>whether a message could be sent</returns>
    public Task<bool> SendPlayer(
        long playerId,
        ArraySegment<byte> content,
        WebSocketMessageType messageType = WebSocketMessageType.Text,
        bool isEndOfMessage = true);

    /// <summary>
    /// send a message to an entire game
    /// </summary>
    /// <param name="gameId">the games id</param>
    /// <param name="content">the content of the message</param>
    /// <param name="messageType">the message type</param>
    /// <param name="isEndOfMessage">bool indicating whether this is the end of a message</param>
    /// <returns>whether a message could be sent</returns>
    public Task<bool> BroadcastGame(
        long gameId,
        ArraySegment<byte> content,
        WebSocketMessageType messageType = WebSocketMessageType.Text,
        bool isEndOfMessage = true);
}