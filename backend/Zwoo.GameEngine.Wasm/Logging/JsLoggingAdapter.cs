using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Wasm.Logging;

[SupportedOSPlatform("browser")]
public partial class WasmLoggerFactory : ILoggerFactory
{
    public static readonly WasmLoggerFactory Instance = new WasmLoggerFactory();

    public WasmLoggerFactory() { }


    #region Interface Implementation

    private void logDebugWrapper(string msg) => _logDebug(msg);
    private void logInfoWrapper(string msg) => _logInfo(msg);
    private void logWarnWrapper(string msg) => _logWarn(msg);
    private void logErrorWrapper(string msg) => _logError(msg);

    public ILogger CreateLogger(string name)
    {
        return new WasmLogger(name)
        {
            _logDebug = logDebugWrapper,
            _logInfo = logInfoWrapper,
            _logWarn = logWarnWrapper,
            _logError = logErrorWrapper,
        };
    }

    #endregion Interface Implementation

    #region Javascript Adaptation

    private Action<string> _logDebug = (string msg) => { };
    private Action<string> _logInfo = (string msg) => { };
    private Action<string> _logWarn = (string msg) => { };
    private Action<string> _logError = (string msg) => { };

    [JSExport]
    public static void OnDebug([JSMarshalAs<JSType.Function<JSType.String>>] Action<string> callback)
    {
        Instance._logDebug = callback;
    }

    [JSExport]
    public static void OnInfo([JSMarshalAs<JSType.Function<JSType.String>>] Action<string> callback)
    {
        Instance._logInfo = callback;
    }

    [JSExport]
    public static void OnWarn([JSMarshalAs<JSType.Function<JSType.String>>] Action<string> callback)
    {
        Instance._logWarn = callback;
    }

    [JSExport]
    public static void OnError([JSMarshalAs<JSType.Function<JSType.String>>] Action<string> callback)
    {
        Instance._logError = callback;
    }

    #endregion Javascript Adaptation
}

public class WasmLogger : ILogger
{
    private readonly string _name;
    internal Action<string> _logDebug = (string msg) => { };
    internal Action<string> _logInfo = (string msg) => { };
    internal Action<string> _logWarn = (string msg) => { };
    internal Action<string> _logError = (string msg) => { };

    public WasmLogger(string name)
    {
        _name = name;
    }

    public void Debug(string msg)
    {
        _logDebug($"[{_name}] {msg}");
    }

    public void Error(string msg)
    {
        _logError($"[{_name}] {msg}");
    }

    public void Info(string msg)
    {
        _logInfo($"[{_name}] {msg}");
    }

    public void Warn(string msg)
    {
        _logWarn($"[{_name}] {msg}");
    }
}
