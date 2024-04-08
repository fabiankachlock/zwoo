using Zwoo.BotDashboard.Distributor;

namespace Zwoo.GameEngine;

public sealed class DebuggingAdapter
{
    private static DebuggingAdapter _instance = new DebuggingAdapter();
    private IDistributor _distributor;

    private DebuggingAdapter()
    {
        _distributor = DistributorFactory.CreateDistributorAsync(Configuration.FromEnv(), true);
    }

    public static async Task Send(OutgoingMessage outgoingMessage)
    {
        await _instance._distributor.Send(outgoingMessage);
    }
}