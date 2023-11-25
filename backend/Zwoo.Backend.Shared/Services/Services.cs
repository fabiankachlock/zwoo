using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Zwoo.Backend.Shared.Services;


public static class ServicesExtensions
{
    public static void AddZwooServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<ILanguageService, LanguageService>();
        services.AddSingleton<ICaptchaService, CaptchaService>();

        services.AddHostedService<EmailService>();
    }
}