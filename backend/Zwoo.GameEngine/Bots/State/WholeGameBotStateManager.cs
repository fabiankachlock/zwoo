using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Bots.State;

/// <summary>
/// a bot state manager managing the bots state while also keeping track of other players
/// </summary>
internal class WholeGameBotStateManager
{
    private BasicBotStateManager _basicState;
    private Dictionary<long, int> _otherPlayers;
    private int? _currentDraw;

    public BasicBotStateManager.BotState GetState() => _basicState.GetState();
    public Dictionary<long, int> GetOtherPlayers() => _otherPlayers;
    public int? GetCurrentDraw() => _currentDraw;

    internal WholeGameBotStateManager()
    {
        _basicState = new BasicBotStateManager();
        _otherPlayers = new Dictionary<long, int>();
        _currentDraw = null;
    }


    internal void AggregateNotification(BotZRPNotification<object> message)
    {
        _basicState.AggregateNotification(message);

        switch (message.Code)
        {
            case ZRPCode.StateUpdated:
                var state = (StateUpdateNotification)message.Payload;
                _currentDraw = state.CurrentDrawAmount;
                foreach (var player in state.CardAmounts)
                {
                    _otherPlayers[player.Key] = player.Value;
                }
                break;
            case ZRPCode.GameStarted:
                var data = (GameStartedNotification)message.Payload;
                foreach (var player in data.Players)
                {
                    _otherPlayers[player.Id] = player.Cards;
                }
                break;
        }
    }

    /// <summary>
    /// reset the current state
    /// </summary>
    internal void Reset()
    {
        _basicState.Reset();
        _otherPlayers.Clear();
        _currentDraw = null;
    }
}