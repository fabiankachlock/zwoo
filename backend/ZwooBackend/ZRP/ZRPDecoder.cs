using System;
using System.Text.Json;

namespace ZwooBackend.ZRP;

public class ZRPDecoder
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static public (ZRPCode?, T?) Decode<T>(string msg)
    {
        int index = msg.IndexOf(",");
        int code = Convert.ToInt32(msg.Substring(0, index));
        string payload = msg.Substring(index + 1);
        return (Enum.IsDefined(typeof(ZRPCode), code) ? (ZRPCode)code : null, JsonSerializer.Deserialize<T>(payload, _options));
    }
}
