using Zwoo.GameEngine.Logging;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Bots.JS;

/// <summary>
/// A wrapper for a delegate that make it available for the javascript engine
/// 
/// This wrapper is specifically built for the bot on event callback to send zrp events
/// </summary>
public class DelegateWrapper
{

    private readonly Action<ZRPCode, object> _action;
    private readonly ILogger _logger;

    public DelegateWrapper(Action<ZRPCode, object> action, ILogger logger)
    {
        _action = action;
        _logger = logger;
    }

    public void Invoke(ZRPCode code, object payload)
    {
        _logger.Info("??? Invoking event in wrapper: " + code);
        _action(code, payload);
        _logger.Info("??? AFTER Invoking event in wrapper: " + code);
    }
}