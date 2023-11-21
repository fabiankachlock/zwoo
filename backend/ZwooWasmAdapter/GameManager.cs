using Zwoo.GameEngine;
using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Logging;
using ZwooWasm.Logging;
using System.Runtime.Versioning;
using System.Runtime.InteropServices.JavaScript;

namespace ZwooWasm;

[SupportedOSPlatform("browser")]
public partial class GameManager
{
    public readonly static GameManager Instance = new GameManager();

    private Zwoo.GameEngine.GameManager _gameManager;

    private ZwooRoom? _activeGame = null;

    private ILogger _logger;

    public GameManager()
    {
        _gameManager = new Zwoo.GameEngine.GameManager(LocalNotificationAdapter.Instance, LocalGameProfileProvider.Instance, WasmLoggerFactory.Instance);
        _logger = WasmLoggerFactory.Instance.CreateLogger("LocalGameManager");
    }

    [JSExport]
    public static void CreateGame([JSMarshalAs<JSType.String>] string name, [JSMarshalAs<JSType.Boolean>] bool isPublic)
    {
        // cleanup
        if (Instance._activeGame != null)
        {
            Instance._logger.Warn("cleaning up local game");
            Instance._activeGame.Close();
        }

        Instance._logger.Debug("creating game");
        Instance._activeGame = Instance._gameManager.CreateGame(name, isPublic);
        Instance._activeGame.OnClosed += () =>
        {
            Instance._logger.Debug("game closed");
            Instance._activeGame = null;
        };
    }

    [JSExport]
    public static void AddPlayer([JSMarshalAs<JSType.String>] string username)
    {
        // cleanup
        if (Instance._activeGame == null)
        {
            return;
        }

        Instance._logger.Debug("adding local player");
        Instance._activeGame.Lobby.Initialize(Constants.LocalUser, username, "", !Instance._activeGame.Game.IsPublic);
        Instance._activeGame.Lobby.IsPlayerAllowedToConnect(Constants.LocalUser);
        Instance._activeGame.Lobby.MarkPlayerConnected(Constants.LocalUser);
    }

    [JSExport]
    public static void CloseGame()
    {
        if (Instance._activeGame != null)
        {
            Instance._logger.Debug("closing game");
            Instance._activeGame.Close();
        }
        Instance._activeGame = null;
    }

    [JSExport]
    public static void SendEvent([JSMarshalAs<JSType.String>] string msg)
    {
        if (Instance._activeGame != null)
        {
            Instance._logger.Debug($"received {msg}");
            ZRPCode? code = ZRPDecoder.GetCode(msg);
            if (code != null)
            {
                LocalEvent zrpMessage = new LocalEvent(code.Value, msg);
                Instance._activeGame.EventDistributer.DistributeEvent(zrpMessage);
            }
            else
            {
                Instance._logger.Warn($"received invalid ZRP message");
            }
        }
    }
}