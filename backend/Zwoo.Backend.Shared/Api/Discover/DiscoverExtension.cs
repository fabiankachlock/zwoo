using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Zwoo.Backend.Shared.Api.Discover;

public static partial class AppExtensions
{
    public static void UseDiscover(this IEndpointRouteBuilder app)
    {
        DiscoverEndpoint.Map(app);
    }
}