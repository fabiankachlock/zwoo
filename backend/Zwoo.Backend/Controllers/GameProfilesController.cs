using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Controllers.DTO;
using Zwoo.Backend.Services.GameProfiles;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Authentication;

namespace Zwoo.Backend.Controllers;

[ApiController]
[Route("game-profiles")]
public class GameProfilesController : Controller
{
    private readonly IGameProfileProvider _gameProfileProvider;

    public GameProfilesController(IGameProfileProvider gameProfileProvider)
    {
        _gameProfileProvider = gameProfileProvider;
    }

    [HttpGet("")]
    public IActionResult GetGameProfiles()
    {
        var activeSession = HttpContext.GetActiveUser();
        var profiles = _gameProfileProvider.GetConfigsOfPlayer(activeSession.User);
        var userProfiles = profiles.Select(profile =>
            new GameProfileResponse(profile.Id, profile.Name, GameProfileGroup.User, profile.Settings));
        var systemProfiles = SystemGameProfiles.All.Select(profile =>
            new GameProfileResponse(profile.Id, profile.Name, GameProfileGroup.System, profile.Settings));
        return Ok(new GameProfileListResponse(
            systemProfiles.Concat(userProfiles).ToList()));
    }
    
    [HttpPost("")]
    public IActionResult CreateGameProfile([FromBody] CreateGameProfile createGameProfile)
    {
        var activeSession = HttpContext.GetActiveUser();
        if (activeSession.User.GameProfiles.Any(profile => profile.Name == createGameProfile.Name))
        {
            return this.BadRequest(ApiError.GameProfileNameExists, "Cannot create profile", "A profile with this name already exists.");
        }
        
        _gameProfileProvider.SaveConfig(activeSession.User, createGameProfile.Name, createGameProfile.Settings);
        return Created();
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateGameProfile(string id, [FromBody] CreateGameProfile createGameProfile)
    {
        var activeSession = HttpContext.GetActiveUser();
        var profile = activeSession.User.GameProfiles.FirstOrDefault(p => p.Id.ToString() == id);
        if (profile == null)
        {
            return this.NotFound(ApiError.GameProfileNotFound, "Profile not found", "The specified profile does not exist.");
        }
        
        _gameProfileProvider.UpdateConfig(activeSession.User, id, createGameProfile.Name,  createGameProfile.Settings);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteGameProfile(string id)
    {
        var activeSession = HttpContext.GetActiveUser();
        var profile = activeSession.User.GameProfiles.FirstOrDefault(p => p.Id.ToString() == id);
        if (profile == null)
        {
            return this.NotFound(ApiError.GameProfileNotFound, "Profile not found", "The specified profile does not exist.");
        }
        
        _gameProfileProvider.DeleteConfig(activeSession.User, id);
        return Ok();
    }
}