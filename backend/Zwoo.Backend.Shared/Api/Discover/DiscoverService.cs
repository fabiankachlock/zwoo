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

    public BetaDiscoverService(ZwooOptions options)
    {
        _options = options;
        _versionOverride = Environment.GetEnvironmentVariable("ZWOO_VERSION_OVERRIDE");
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