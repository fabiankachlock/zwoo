using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Zwoo.GameEngine.ZRP;

public class ZRPDecoder
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    static public (ZRPCode?, T?) Decode<T>(string msg) where T : class
    {
        int index = msg.IndexOf(",");
        int code = Convert.ToInt32(msg.Substring(0, index));
        string payload = msg.Substring(index + 1);
        _options.TypeInfoResolverChain.Insert(0, new ZRPSerializerContext());
        var result = JsonSerializer.Deserialize(payload, typeof(T), new ZRPSerializerContext(_options));
        ZRPCode? resultCode = Enum.IsDefined(typeof(ZRPCode), code) ? (ZRPCode)code : null;
        return (resultCode, result as T);
    }

    static public (ZRPCode?, T?) DecodeFromBytes<T>(byte[] msg) where T : class
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

    static public T? DecodePayload<T>(string msg) where T : class
    {
        int index = msg.IndexOf(",");
        string payload = msg.Substring(index + 1);
        var result = JsonSerializer.Deserialize(payload, typeof(T), new ZRPSerializerContext(_options));
        return result as T;
    }

    static public T? DecodePayloadFromBytes<T>(byte[] msg) where T : class
    {
        return DecodePayload<T>(Encoding.UTF8.GetString(msg));
    }
}
