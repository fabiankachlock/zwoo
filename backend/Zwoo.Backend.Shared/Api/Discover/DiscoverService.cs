using Microsoft.Extensions.Options;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Configuration;

namespace Zwoo.Backend.Shared.Api.Discover;

public interface IDiscoverService
{
    public bool CanConnect(ClientInfo client);
}

public class BetaDiscoverService : IDiscoverService
{

    private ZwooOptions _options;

    public BetaDiscoverService(IOptions<ZwooOptions> options)
    {
        _options = options.Value;
    }

    public bool CanConnect(ClientInfo client)
    {
        // only allow same version connects
        return _options.App.AppVersion == client.Version;
    }
}