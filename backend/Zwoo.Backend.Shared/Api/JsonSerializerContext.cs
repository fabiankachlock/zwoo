using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Authentication;

namespace Zwoo.Backend.Shared.Api;

[JsonSerializable(typeof(ClientInfo))]
[JsonSerializable(typeof(ContactForm))]
[JsonSerializable(typeof(CreateGame))]
[JsonSerializable(typeof(GameMeta))]
[JsonSerializable(typeof(GamesList))]
[JsonSerializable(typeof(JoinGame))]
[JsonSerializable(typeof(JoinedGame))]
[JsonSerializable(typeof(UserSession))]
[JsonSerializable(typeof(Login))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(IResult))]
[JsonSerializable(typeof(Task<IResult>))]
[JsonSerializable(typeof(Task))]
public partial class ApiJsonSerializerContext : JsonSerializerContext
{

}