
namespace Zwoo.GameEngine.Game.Events;

public struct GameInterrupt
{
    internal readonly string OriginRule;
    internal readonly string Reason;
    internal readonly List<long> TargetPlayers;

    internal GameInterrupt(string originRule, string reason, List<long> targetPlayers)
    {
        OriginRule = originRule;
        Reason = reason;
        TargetPlayers = targetPlayers;
    }
}

public struct InterruptPayload
{
    internal readonly List<long> TargetPlayers;

    internal InterruptPayload(List<long> targetPlayers)
    {
        TargetPlayers = targetPlayers;
    }
}
