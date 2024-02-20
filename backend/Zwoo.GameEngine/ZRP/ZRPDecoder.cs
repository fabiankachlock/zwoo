using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Zwoo.GameEngine.ZRP;

public class ZRPDecoder
{
    private static ZRPSerializerContext _context = new ZRPSerializerContext(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });

    static public (ZRPCode?, T?) Decode<T>(string msg)
    {
        int index = msg.IndexOf(",");
        int code = Convert.ToInt32(msg.Substring(0, index));
        string payload = msg.Substring(index + 1);
        var result = JsonSerializer.Deserialize(payload, typeof(T), _context);
        ZRPCode? resultCode = Enum.IsDefined(typeof(ZRPCode), code) ? (ZRPCode)code : null;
        return (resultCode, (T?)result);
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
        var result = JsonSerializer.Deserialize(payload, typeof(T), _context);
        return (T?)result;
    }

    static public T? DecodePayloadFromBytes<T>(byte[] msg)
    {
        return DecodePayload<T>(Encoding.UTF8.GetString(msg));
    }
}
