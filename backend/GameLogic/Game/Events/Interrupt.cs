
namespace ZwooGameLogic.Game.Events;

internal struct GameInterrupt
{
    internal readonly string OriginRule;
    internal readonly string Reason;
    internal readonly List<ulong> TargetPlayers;

    internal GameInterrupt(string originRule, string reason, List<ulong> targetPlayers)
    {
        OriginRule = originRule;
        Reason = reason;
        TargetPlayers = targetPlayers;
    }
}

internal struct InterruptPayload
{
    internal readonly List<ulong> TargetPlayers;

    internal InterruptPayload(List<ulong> targetPlayers)
    {
        TargetPlayers = targetPlayers;
    }
}
