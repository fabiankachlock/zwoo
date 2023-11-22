using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api.Discover;

public static partial class AppExtensions
{
    public static void UseDiscover(this WebApplication app)
    {
        DiscoverEndpoint.Map(app);
    }
}