using System;
using System.Text.Json;

namespace ZwooBackend.ZRP;

public class ZRPEncoder
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static public string Encode<T>(ZRPCode code, T payload)
    {
        return $"{code},{JsonSerializer.Serialize(payload, ZRPEncoder._options)}";
    }
}
