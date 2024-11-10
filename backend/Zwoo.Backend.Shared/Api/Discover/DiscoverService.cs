using Microsoft.Extensions.Logging;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Configuration;

namespace Zwoo.Backend.Shared.Api.Discover;

public interface IDiscoverService
{
    public ClientInfo GetVersion();
    public bool CanConnect(ClientInfo client);
}

public class BetaDiscoverService : IDiscoverService
{

    private ZwooOptions _options;
    private readonly string? _versionOverride;

    public BetaDiscoverService(ZwooOptions options, ILogger<BetaDiscoverService> logger)
    {
        _options = options;
        var rawOverride = Environment.GetEnvironmentVariable("ZWOO_VERSION_OVERRIDE");
        _versionOverride = string.IsNullOrEmpty(rawOverride) ? null : rawOverride;
        logger.LogInformation("Server version: {version}", _options.App.AppVersion);
        logger.LogInformation("Server mode: {mode}", _options.App.ServerMode);
        logger.LogInformation("Server hash: {hash}", _options.App.AppVersionHash);
        logger.LogInformation("Server version override: {version}", _versionOverride);
        logger.LogInformation("Server ZRP Version: {hash}", _options.App.ZRPVersion);
    }

    public bool CanConnect(ClientInfo client)
    {
        // only allow same version connects
        if (_versionOverride != null)
        {
            return _versionOverride == client.Version && client.Mode == "online" && _options.App.ZRPVersion == client.ZRPVersion;
        }

        return _options.App.AppVersion == client.Version && _options.App.ServerMode == client.Mode && _options.App.ZRPVersion == client.ZRPVersion;
    }

    public ClientInfo GetVersion()
    {
        return new ClientInfo()
        {
            Version = _versionOverride ?? _options.App.AppVersion,
            Hash = _options.App.AppVersionHash,
            Mode = _options.App.ServerMode,
            ZRPVersion = _options.App.ZRPVersion,
        };
    }
}