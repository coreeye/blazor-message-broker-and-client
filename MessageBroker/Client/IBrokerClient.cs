﻿using System;
using System.Net.WebSockets;
using System.Threading;

namespace MessageBroker.Client
{
    internal interface IBrokerClient : IDisposable
    {
        ClientWebSocket WebSocket { get; }

        void Connect(Uri uri, Action onConnectCallback = null, Action<string> logCallback = null);

        void MakeCreateTopicRequest(string topicName);

        void MakeSubscriptionRequest(string topicName);

        void MakeListTopicsRequest();

        void MakeUnsubscriptionRequest(string topicName);

        void MakePublishRequest<T>(T messageContent);
    }
}