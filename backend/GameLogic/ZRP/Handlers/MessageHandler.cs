using ZwooBackend.ZRP;
using ZwooBackend.Websockets.Interfaces;

namespace ZwooBackend.Websockets.Handlers;

public interface MessageHandler
{
    bool HandleMessage(UserContext context, ZRPMessage message);
}
