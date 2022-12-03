using Prism.Events;

namespace Client.Events
{
    internal class ConnectionStatusChanged : PubSubEvent<string>
    {
    }
}
