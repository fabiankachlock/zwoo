using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static ZwooBackend.Globals;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")] 
[Route("")]
public class MiscController : Controller
{
    [HttpGet("version")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetVersion()
    {
        return Ok(Globals.Version);
    }
    
    [HttpGet("changelog")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public IActionResult GetChangelog([FromQuery] string version)
    {
        var changelog = ZwooDatabase.GetChangelog(version);
        if (changelog == null)
            return NotFound("");
        return  Ok(changelog.ChangelogText);
    }
    
    [HttpGet("versionHistory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetChangelogs() => Ok($"{{ \"versions\": {JsonSerializer.Serialize(ZwooDatabase.GetChangelogs().Select(c => c.ChangelogVersion))} }}");
}