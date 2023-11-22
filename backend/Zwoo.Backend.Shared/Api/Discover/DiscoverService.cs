using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api.Discover;

public interface IDiscoverService
{
    public bool CanConnect(ClientInfo client);
}