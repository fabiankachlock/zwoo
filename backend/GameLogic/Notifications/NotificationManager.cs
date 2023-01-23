
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Notifications;

/// <summary>
/// an object that forwards notifications to targets
/// </summary>
public class NotificationManager : INotificationAdapter
{

    private List<INotificationTarget> _targets;
    private ILogger _logger;

    public NotificationManager(ILogger logger)
    {
        _targets = new List<INotificationTarget>();
        _logger = logger;
    }

    public void AddTarget(INotificationTarget target)
    {
        _targets.Add(target);
    }

    public bool RemoveTarget(INotificationTarget target)
    {
        return _targets.Remove(target);
    }

    #region Interface Implementation

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        _logger.Debug($"[Player] [{playerId}] sending {code} {payload}");
        var target = _targets.Find(target => target.PlayerId == playerId);
        if (target != null)
        {
            target.ReceiveMessage(code, payload);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        _logger.Debug($"[Game] [{gameId}] broadcasting");
        var targets = _targets.FindAll(target => target.GameId == gameId);
        if (targets != null)
        {
            targets.ForEach(target => target.ReceiveMessage(code, payload));
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> DisconnectPlayer(long playerId)
    {
        _logger.Debug($"[Player] [{playerId}] disconnecting");
        var target = _targets.Find(target => target.PlayerId == playerId);
        if (target != null)
        {
            target.ReceiveDisconnect();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> DisconnectGame(long gameId)
    {
        _logger.Debug($"[Game] [{gameId}] disconnecting");
        var targets = _targets.FindAll(target => target.GameId == gameId);
        if (targets != null)
        {
            targets.ForEach(target => target.ReceiveDisconnect());
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    #endregion Interface Implementation
}
