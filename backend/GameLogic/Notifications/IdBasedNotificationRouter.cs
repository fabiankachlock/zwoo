using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Notifications;

public interface IRoutableNotificationAdapter : INotificationAdapter
{
    public long TargetId { get; }
}

/// <summary>
/// a notification adapter that distributes messages to multiple receiver notification adapters based on target ids
/// 
/// USECASE: wsconnection/bot splitting;
/// 
/// this sends by default all messages to the default adapter, whilst translating the lobby id to the real id
/// if there is an receiver for an real id he gets that message, keeping the lobby id of the receiver 
/// </summary>
public class IdBasedNotificationRouter : INotificationAdapter
{
    private readonly ZwooRoom _room;
    private readonly INotificationAdapter _defaultTarget;
    private readonly List<IRoutableNotificationAdapter> _targets;

    public IdBasedNotificationRouter(ZwooRoom room, INotificationAdapter defaultTarget)
    {
        _room = room;
        _defaultTarget = defaultTarget;
        _targets = new();
    }

    public void RegisterTarget(IRoutableNotificationAdapter target)
    {
        _targets.Add(target);
    }

    public void RemoveTarget(IRoutableNotificationAdapter target)
    {
        _targets.Remove(target);
    }

    public async Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        var result = await Task.WhenAll(_targets.Select(target => target.BroadcastGame(gameId, code, payload)));
        var defaultResult = await _defaultTarget.BroadcastGame(gameId, code, payload);
        return !result.Contains(false) && defaultResult;
    }

    public async Task<bool> DisconnectGame(long gameId)
    {
        var result = await Task.WhenAll(_targets.Select(target => target.DisconnectGame(gameId)));
        var defaultResult = await _defaultTarget.DisconnectGame(gameId);
        return !result.Contains(false) && defaultResult;
    }

    public async Task<bool> DisconnectPlayer(long playerId)
    {
        var player = _room.GetPlayer(playerId);
        if (player == null) return false;

        foreach (var target in _targets)
        {
            if (target.TargetId == player.RealId)
            {
                return await target.DisconnectPlayer(player.LobbyId);
            }
        }
        return await _defaultTarget.DisconnectPlayer(playerId);
    }

    public async Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        var player = _room.GetPlayer(playerId);
        if (player == null) return false;

        foreach (var target in _targets)
        {
            if (target.TargetId == player.RealId)
            {
                return await target.SendPlayer(player.LobbyId, code, payload);
            }
        }
        return await _defaultTarget.SendPlayer(playerId, code, payload);
    }
}