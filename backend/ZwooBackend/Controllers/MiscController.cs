using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")] 
[Route("")]
public class MiscController : Controller
{
    [Microsoft.AspNetCore.Mvc.HttpGet("version")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetVersion()
    {
        return Ok("1.0.0-alpha.5");
    }
}