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
[Route("changelog")]
public class ChangelogController : Controller
{
    private IChangelogService _changelogs;

    public ChangelogController(IChangelogService changelogs)
    {
        _changelogs = changelogs;
    }

    [HttpGet("")]
    public IActionResult GetChangelogs()
    {
        var versions = _changelogs.GetChangelogs().Select(c => c.ChangelogVersion);
        return Ok(new VersionHistory()
        {
            Versions = versions.ToList(),
        });
    }

    [HttpGet("{version}")]
    public IActionResult GetChangelog([FromRoute(Name = "version")] string version)
    {
        var changelog = _changelogs.GetChangelog(version);
        if (changelog == null)
        {
            return NotFound(new ProblemDetails()
            {
                Title = "Changelog not found",
                Detail = "A changelog with the provided version cannot be found.",
                Instance = HttpContext.Request.Path
            });
        }
        return Ok(changelog.ChangelogText);
    }
}