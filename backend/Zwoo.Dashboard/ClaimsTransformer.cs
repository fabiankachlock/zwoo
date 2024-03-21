using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Zwoo.Dashboard.Data;

namespace Zwoo.Dashboard;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    private readonly AuthOptions _options;

    public KeycloakRolesClaimsTransformation(IOptions<ZiadOptions> options)
    {
        _options = options.Value.Auth;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var result = principal.Clone();
        if (result.Identity is not ClaimsIdentity identity)
        {
            return Task.FromResult(result);
        }

        var resourceAccessValue = principal.FindFirst("resource_access")?.Value;
        if (string.IsNullOrWhiteSpace(resourceAccessValue))
        {
            return Task.FromResult(result);
        }

        try
        {
            // parsing "resource_access.${client_id}.roles
            using var resourceAccess = JsonDocument.Parse(resourceAccessValue);
            var clientRoles = resourceAccess
                .RootElement
                .GetProperty(_options.ClientID)
                .GetProperty("roles");

            foreach (var role in clientRoles.EnumerateArray())
            {
                var value = role.GetString();
                if (!String.IsNullOrWhiteSpace(value))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, value));
                }
            }

        }
        catch { }

        return Task.FromResult(result);
    }
}