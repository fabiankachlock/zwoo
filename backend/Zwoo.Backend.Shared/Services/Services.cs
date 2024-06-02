using Microsoft.Extensions.DependencyInjection;
using Zwoo.Backend.Shared.Api.Discover;

namespace Zwoo.Backend.Shared.Services;


public static class ServicesExtensions
{
    public static void AddZwooServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<ILanguageService, LanguageService>();
        services.AddSingleton<ICaptchaService, CaptchaService>();
        services.AddSingleton<IDiscoverService, BetaDiscoverService>();

        services.AddHostedService<EmailService>();
    }
}