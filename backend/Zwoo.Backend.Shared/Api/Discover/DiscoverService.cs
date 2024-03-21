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

    public BetaDiscoverService(ZwooOptions options)
    {
        _options = options;
    }

    public bool CanConnect(ClientInfo client)
    {
        // only allow same version connects
        return _options.App.AppVersion == client.Version && _options.App.ServerMode == client.Mode;
    }

    public ClientInfo GetVersion()
    {
        return new ClientInfo()
        {
            Version = _options.App.AppVersion,
            Mode = _options.App.ServerMode,
            ZRPVersion = "",
        };
    }
}