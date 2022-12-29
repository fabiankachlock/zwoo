using ZwooGameLogic;
using System;
using System.Runtime.InteropServices.JavaScript;

namespace ZwooWasm;

public partial class GameManager
{
    // private ZwooGameLogic.GameManager _gameManager;

    public GameManager()
    {
        // _gameManager = new ZwooGameLogic.GameManager();
    }

    [JSExport]
    public static void Test([JSMarshalAsAttribute<JSType.Function<JSType.Number, JSType.Number>>] Func<int, int> callback)
    {
        int ex = 10;
        Console.WriteLine($"before: {ex}");
        ex = callback(ex);
        Console.WriteLine($"after: {ex}");
    }
}