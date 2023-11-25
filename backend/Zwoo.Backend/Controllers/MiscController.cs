using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Controllers.DTO;
using Zwoo.Backend.Services;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database;
using static Zwoo.Backend.Globals;

namespace Zwoo.Backend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("")]
public class MiscController : Controller
{
    private IEmailService _emailService;
    private IContactRequestService _contactRequests;
    private IChangelogService _changelogs;
    private ICaptchaService _captcha;

    public MiscController(IEmailService emailService, IChangelogService changelogs, IContactRequestService contactRequests, ICaptchaService captcha)
    {
        _emailService = emailService;
        _changelogs = changelogs;
        _contactRequests = contactRequests;
        _captcha = captcha;
    }

    [HttpGet("version")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetVersion()
    {
        return Ok(Globals.ApiVersion);
    }

    [HttpGet("changelog")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult GetChangelog([FromQuery] string version)
    {
        var changelog = _changelogs.GetChangelog(version);
        if (changelog == null)
        {
            return NotFound("changelog for version not present");
        }
        return Ok(changelog.ChangelogText);
    }

    [HttpGet("versionHistory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VersionHistory))]
    public IActionResult GetChangelogs()
    {
        var versions = _changelogs.GetChangelogs().Select(c => c.ChangelogVersion);
        return Ok(new VersionHistory()
        {
            Versions = versions.ToList(),
        });
    }

    [EnableCors("ContactForm")]
    [HttpPost("contactForm")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> SubmitContactForm([FromBody] ContactForm body)
    {
        var captchaResponse = await _captcha.Verify(body.CaptchaToken);
        if (captchaResponse == null || !captchaResponse.Success)
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.CAPTCHA_INVALID, "Operation needs valid captcha token"));
        }

        var request = new Zwoo.Database.Dao.ContactRequest()
        {
            Name = body.Name,
            Email = body.Email,
            Message = body.Message,
            Origin = body.Site,
            CaptchaScore = captchaResponse.Score,
        };
        request = _contactRequests.CreateRequest(request);
        if (request.CaptchaScore >= 0.5)
        {
            _emailService.SendContactFormEmail(_emailService.CreateRecipient(ContactEmail, "Zwoo.Backend", LanguageCode.English), request);
        }
        else
        {
            Globals.Logger.Info("skip sending contact email because captcha score is too low");
        }
        return Ok("sent!");
    }
}