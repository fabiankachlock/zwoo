using ZwooGameLogic;
using ZwooGameLogic.ZRP;
using ZwooWasm.Logging;
using System;
using System.Runtime.InteropServices.JavaScript;

namespace ZwooWasm;

public partial class GameManager
{
    public readonly static GameManager Instance = new GameManager();

    private ZwooGameLogic.GameManager _gameManager;

    private ZwooRoom? _activeGame = null;

    public GameManager()
    {
        _gameManager = new ZwooGameLogic.GameManager(LocalNotificationAdapter.Instance, WasmLoggerFactory.Instance);
    }

    [JSExport]
    public static void CreateGame([JSMarshalAs<JSType.String>] string name, [JSMarshalAs<JSType.Boolean>] bool isPublic)
    {
        // cleanup
        if (Instance._activeGame != null)
        {
            Instance._activeGame.Close();
        }

        Instance._activeGame = Instance._gameManager.CreateGame(name, isPublic);
        Instance._activeGame.OnClosed += () =>
        {
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
        Console.WriteLine("### adding local player");
        Instance._activeGame.Lobby.Initialize(Constants.LocalUser, username, "", !Instance._activeGame.Game.IsPublic);
        Instance._activeGame.Lobby.IsPlayerAllowedToConnect(Constants.LocalUser);
    }

    [JSExport]
    public static void CloseGame()
    {
        if (Instance._activeGame != null)
        {
            Instance._activeGame.Close();
        }
        Instance._activeGame = null;
    }

    [JSExport]
    public static void SendEvent([JSMarshalAs<JSType.Number>] int code, [JSMarshalAs<JSType.Any>] object payload)
    {
        if (Instance._activeGame != null)
        {
            LocalEvent evt = new LocalEvent((ZRPCode)code, payload);
            Instance._activeGame.DistributeEvent(evt);
        }
    }
}