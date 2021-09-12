using System;

namespace MessageBroker.Client
{
    public interface IAppConfig
    {
        Uri BrokerAddress { get; }
    }
}