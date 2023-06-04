using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Services;
using ZwooDatabase;
using static ZwooBackend.Globals;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("")]
public class MiscController : Controller
{
    private IEmailService _emailService;
    private IChangelogService _changelogs;

    public MiscController(IEmailService emailService, IChangelogService changelogs)
    {
        _emailService = emailService;
        _changelogs = changelogs;
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
    public IActionResult SubmitContactForm([FromBody] ContactForm body)
    {
        var request = Globals.ZwooDatabase.CreateContactRequest(body);
        if (body.CaptchaScore >= 0.5)
        {
            _emailService.SendContactFormEmail(_emailService.CreateRecipient("info@igd20.de", "ZwooBackend", LanguageCode.English), request);
        }
        else
        {
            Globals.Logger.Info("skip sending contact email because captcha score is too low");
        }
        return Ok("sent!");
    }
}