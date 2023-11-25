using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Zwoo.Backend.Shared.Services;


public static class ServicesExtensions
{
    public static void AddZwooServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddSingleton<ILanguageService, LanguageService>();
        builder.Services.AddSingleton<ICaptchaService, CaptchaService>();

        builder.Services.AddHostedService<EmailService>();
    }
}