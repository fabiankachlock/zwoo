using log4net;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    [HttpPost("create")]
    public string CreateAccount([FromBody] CreateAccount body)
    {
        Globals.Logger.Info(body.username);
        return "";
    }
}