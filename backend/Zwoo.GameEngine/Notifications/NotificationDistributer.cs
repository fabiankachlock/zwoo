using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Notifications;

/// <summary>
/// a notification adapter which distributes a notification to a number of other notification adapters
/// </summary>
public class NotificationDistributer : INotificationAdapter
{

    private List<INotificationAdapter> _targets;

    public NotificationDistributer(params INotificationAdapter[] targets)
    {
        _targets = new List<INotificationAdapter>(targets);
    }

    public void AddTarget(INotificationAdapter target)
    {
        _targets.Add(target);
    }

    public bool RemoveTarget(INotificationAdapter target)
    {
        return _targets.Remove(target);
    }

    #region Interface Implementation 

    public async Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        var result = await Task.WhenAll(_targets.Select(target => target.SendPlayer(playerId, code, payload)));
        return !result.Contains(false);
    }

    public async Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        var result = await Task.WhenAll(_targets.Select(target => target.BroadcastGame(gameId, code, payload)));
        return !result.Contains(false);
    }

    public async Task<bool> DisconnectPlayer(long playerId)
    {
        var result = await Task.WhenAll(_targets.Select(target => target.DisconnectPlayer(playerId)));
        return !result.Contains(false);
    }

    public async Task<bool> DisconnectGame(long gameId)
    {
        var result = await Task.WhenAll(_targets.Select(target => target.DisconnectGame(gameId)));
        return !result.Contains(false);
    }

    #endregion Interface Implementation 
}