using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using ZwooInfoDashBoard.Data;

namespace ZwooInfoDashBoard;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public KeycloakRolesClaimsTransformation() { }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var result = principal.Clone();
        if (result.Identity is not ClaimsIdentity identity)
        {
            return Task.FromResult(result);
        }

        var resourceAccessValue = principal.FindFirst("resource_access")?.Value;
        if (String.IsNullOrWhiteSpace(resourceAccessValue))
        {
            return Task.FromResult(result);
        }

        try
        {
            // parsing "resource_access.${client_id}.roles
            using var resourceAccess = JsonDocument.Parse(resourceAccessValue);
            var clientRoles = resourceAccess
                .RootElement
                .GetProperty(Globals.AuthenticationClientId)
                .GetProperty("roles");

            Console.WriteLine(clientRoles);

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