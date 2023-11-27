using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Shared.Api.Contact;

public class ContactEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/contact", async ([FromBody] ContactForm body,
            ICaptchaService _captcha,
            IContactRequestService _contactRequests,
            IOptions<ZwooOptions> _options,
            ILogger<ContactEndpoint> _logger,
            IEmailService _emailService) =>
        {
            var captchaResponse = await _captcha.Verify(body.CaptchaToken);
            if (captchaResponse == null || !captchaResponse.Success)
            {
                return Results.BadRequest(ApiError.InvalidCaptcha.ToProblem(new ProblemDetails()
                {
                    Title = "Invalid captcha",
                    Detail = "The captcha challenge must be passed.",
                    Instance = "/contact"
                }));
            }

            var request = new ContactRequest()
            {
                Name = body.Name,
                Email = body.Email,
                Message = body.Message,
                Origin = body.Site,
                CaptchaScore = captchaResponse.Score,
            };

            request = _contactRequests.CreateRequest(request);
            _logger.LogInformation("registered a new contact request");
            if (request.CaptchaScore >= 0.5)
            {
                _emailService.SendContactFormEmail(_emailService.CreateRecipient(_options.Value.Email.ContactEmail, "Zwoo.Backend", LanguageCode.English), request);
            }
            else
            {
                _logger.LogWarning("skip sending contact email because captcha score is too low");
            }
            return Results.Ok();
        });
    }
}