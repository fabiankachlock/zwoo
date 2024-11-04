namespace Zwoo.GameEngine.Bots.JS;

/// <summary>
/// A collection of c# helper exposed to javascript
/// </summary>
public class ScriptHelper
{
    /// <summary>
    /// Cast a arbitrary value to a an integer
    /// </summary>
    /// <param name="value">the JS value</param>
    /// <returns>an c# int</returns>
    public int toInt(dynamic value)
    {
        return (int)value;
    }
}