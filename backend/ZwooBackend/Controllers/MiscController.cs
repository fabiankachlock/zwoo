using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Services;
using static ZwooBackend.Globals;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("")]
public class MiscController : Controller
{
    private IEmailService _emailService;

    public MiscController(IEmailService emailService)
    {
        _emailService = emailService;
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
        var changelog = ZwooDatabase.GetChangelog(version);
        if (changelog == null)
            return NotFound("");
        return Ok(changelog.ChangelogText);
    }

    [HttpGet("versionHistory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetChangelogs() => Ok($"{{ \"versions\": {JsonSerializer.Serialize(ZwooDatabase.GetChangelogs().Select(c => c.ChangelogVersion))} }}");

    [HttpPost("contactForm")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult SubmitContactForm([FromBody] ContactForm body)
    {
        _emailService.SendContactFormEmail(_emailService.CreateRecipient(Globals.SmtpHostEmail, Globals.SmtpUsername, LanguageCode.English), body.Sender, body.Message);
        return Ok("sent!");
    }
}