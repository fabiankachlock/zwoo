﻿using System.Text;
using System.Text.Json;
using ZwooGameLogic.ZRP;

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

    static public (ZRPCode?, T?) DecodeFromBytes<T>(byte[] msg)
    {
        return Decode<T>(Encoding.UTF8.GetString(msg));
    }

    static public ZRPCode? GetCode(string msg)
    {
        int index = msg.IndexOf(",");
        int code = Convert.ToInt32(msg.Substring(0, index));
        return Enum.IsDefined(typeof(ZRPCode), code) ? (ZRPCode)code : null;
    }

    static public ZRPCode? GetCodeFromBytes(byte[] msg)
    {
        return GetCode(Encoding.UTF8.GetString(msg));

    }

    static public T? DecodePayload<T>(string msg)
    {
        int index = msg.IndexOf(",");
        string payload = msg.Substring(index + 1);
        return JsonSerializer.Deserialize<T>(payload, _options);
    }

    static public T? DecodePayloadFromBytes<T>(byte[] msg)
    {
        return DecodePayload<T>(Encoding.UTF8.GetString(msg));
    }
}
