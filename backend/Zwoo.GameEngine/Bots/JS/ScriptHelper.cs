namespace Zwoo.GameEngine.Bots.JS;

/// <summary>
/// A collection of c# helper exposed to javascript
/// </summary>
internal class ScriptHelper
{
    /// <summary>
    /// Cast a arbitrary value to a an integer
    /// </summary>
    /// <param name="value">the JS value</param>
    /// <returns>an c# int</returns>
    public int CastToInt(dynamic value)
    {
        return (int)value;
    }
}