using System;

namespace MessageBroker.Client
{
    public class AppConfig : IAppConfig
    {
        public Uri BrokerAddress { get; private set; } = new Uri("wss://localhost:5001/ws");
    }
}
