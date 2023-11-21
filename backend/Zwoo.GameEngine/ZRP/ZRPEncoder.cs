using System.Text;
using System.Text.Json;

namespace Zwoo.GameEngine.ZRP;

public class ZRPEncoder
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static public string Encode<T>(ZRPCode code, T payload)
    {
        return $"{(int)code},{EncodePayload(payload)}";
    }

    static public string EncodePayload<T>(T payload)
    {
        return JsonSerializer.Serialize(payload, _options);
    }

    static public byte[] EncodeToBytes<T>(ZRPCode code, T payload)
    {
        return Encoding.UTF8.GetBytes(Encode(code, payload));
    }
}
