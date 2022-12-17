using System;

using System.Runtime.InteropServices.JavaScript;

Console.WriteLine("Hello, Browser!");

public partial class HelloWorld
{
    private static Random rnd = new Random();

    [JSExport]
    public static int Test()
    {
        return rnd.Next();
    }
}
