using Microsoft.AspNetCore.Builder;

namespace Zwoo.Backend.Shared.Api.Contact;

public static partial class ContactExtensions
{
    public static void UseContactRequests(this WebApplication app)
    {
        ContactEndpoint.Map(app);
    }
}