using Prism.Events;

namespace Client.Events
{
    internal class StatusBarMessage : PubSubEvent<(bool, string)> // кортеж
    {
    }
}
