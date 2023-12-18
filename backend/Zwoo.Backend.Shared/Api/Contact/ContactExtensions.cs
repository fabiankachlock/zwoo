using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Zwoo.Backend.Shared.Api.Contact;

public static partial class ContactExtensions
{
    public static void UseContactRequests(this IEndpointRouteBuilder app)
    {
        ContactEndpoint.Map(app);
    }
}