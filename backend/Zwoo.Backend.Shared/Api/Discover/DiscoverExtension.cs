using Microsoft.AspNetCore.Builder;

namespace Zwoo.Backend.Shared.Api.Discover;

public static partial class AppExtensions
{
    public static void UseDiscover(this WebApplication app)
    {
        DiscoverEndpoint.Map(app);
    }
}