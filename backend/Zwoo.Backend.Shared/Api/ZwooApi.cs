using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zwoo.Backend.Shared.Configuration;

namespace Zwoo.Backend.Shared.Api;

public static class AppExtensions
{
    public static void AddZwooCors(this WebApplicationBuilder builder, ZwooOptions config)
    {
        builder.Services.AddCors(s =>
        {
            s.AddDefaultPolicy(b => b
                .WithOrigins(config.Server.Cors)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            s.AddPolicy("ContactForm", b => b
                .WithOrigins(config.Server.ContactFormExtraCorsOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
    }

    public static void UseZwooCors(this WebApplication app)
    {
        app.UseCors();
    }
}