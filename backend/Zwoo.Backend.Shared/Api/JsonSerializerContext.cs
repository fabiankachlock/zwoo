using System.Text.Json.Serialization;
using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api;

[JsonSerializable(typeof(ClientInfo))]
[JsonSerializable(typeof(ContactForm))]
[JsonSerializable(typeof(CreateGame))]
[JsonSerializable(typeof(GameMeta))]
[JsonSerializable(typeof(GamesList))]
[JsonSerializable(typeof(JoinGame))]
[JsonSerializable(typeof(JoinedGame))]
public partial class ApiJsonSerializerContext : JsonSerializerContext
{

}