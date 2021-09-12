using MessageBroker.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBroker.Client
{
    /// <summary>
    /// Specialized client to act as a publisher in a publish-subscribe system
    /// </summary>
    public class BrokerClient : IBrokerClient
    {
        public List<NameIdPair> TopicCache { get; private set; }

        public ClientWebSocket WebSocket { get; private set; }

        private ConcurrentQueue<Request> pendingRequests;

        CancellationTokenSource disposalTokenSource = new CancellationTokenSource();
        Action<string> _logCallback;

        public BrokerClient() : base()
        {
            WebSocket = new ClientWebSocket();
            pendingRequests = new ConcurrentQueue<Request>();
            TopicCache = new List<NameIdPair>();
        }

        public async void Connect(Uri uri, Action onConnectCallback = null, Action<string> logCallback = null)
        {
            try
            {
                _logCallback = logCallback;
                WebSocket = new ClientWebSocket();
                await WebSocket.ConnectAsync(uri, disposalTokenSource.Token);
                onConnectCallback?.Invoke();
                _ = ReceiveLoop();
            }
            catch (Exception e)
            {
                onConnectCallback?.Invoke();
                _logCallback("Could not connect.");
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            while (!disposalTokenSource.IsCancellationRequested)
            {
                // Note that the received block might only be part of a larger message. If this applies in your scenario,
                // check the received.EndOfMessage and consider buffering the blocks until that property is true.
                // Or use a higher-level library such as SignalR.
                var received = await WebSocket.ReceiveAsync(buffer, disposalTokenSource.Token);
                var receivedAsText = Encoding.UTF8.GetString(buffer.Array, 0, received.Count);
                _logCallback?.Invoke($"Received: {receivedAsText}\n");
            }
        }

        protected void ProcessResponse(Response response)
        {
            // Handle responses to previous client requests
            switch (response.Type)
            {
                case ResponseType.NEW_MESSAGE:
                    {
                        // Show a newly published message from the broker
                        var message = (response as NewMessageResponse<string>).Message;

                        var logText = $"[New Message: \"{message.Content}\"";

                        _logCallback?.Invoke(logText);

                        break;
                    }

                case ResponseType.LIST_TOPICS:
                    {
                        // Cache the topic list and show the list of topics
                        TopicCache = (response as ListTopicsResponse).Topics;

                        _logCallback?.Invoke($"[Topic List]");
                        int index = 0;
                        foreach (NameIdPair topic in TopicCache)
                        {
                            _logCallback?.Invoke($" {index}. {topic.Name}");
                            index++;
                        }

                        break;
                    }

                case ResponseType.SUBSCRIBED:
                    {
                        // Add the new topic to the list of owned topics
                        var subscriptions = (response as SubscribedResponse).Subscriptions.Select(s => s.Name);

                        _logCallback?.Invoke($"Subscribe success");
                        subscriptions.ToList().ForEach(s => _logCallback?.Invoke($"[Current subscriptions \"{s}\" has been created]"));
                        break;
                    }

                case ResponseType.UNSUBSCRIBED:
                    {
                        // Add the new topic to the list of owned topics
                        var subscriptions = (response as SubscribedResponse).Subscriptions.Select(s => s.Name);

                        _logCallback?.Invoke($"Unsubscribe success");
                        subscriptions.ToList().ForEach(s => _logCallback?.Invoke($"[Current subscriptions \"{s}\" has been created]"));
                        break;
                    }

                case ResponseType.TOPIC_CREATED:
                    {
                        // Add the new topic to the list of owned topics
                        var topicInfo = (response as TopicCreatedResponse).TopicInfo;
                        TopicCache.Add(topicInfo);

                        _logCallback?.Invoke($"[Topic \"{topicInfo.Name}\" has been created]");
                        break;
                    }

                case ResponseType.INFO:
                    _logCallback?.Invoke($"[Info] {(response as InfoResponse).Text}");
                    break;
            }
        }

        private void AddRequest(Request request)
        {
            pendingRequests.Enqueue(request);
        }

        public async void MakeCreateTopicRequest(string topicName)
        {
            var request = new CreateTopicRequest(topicName);
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, disposalTokenSource.Token);
        }

        public async void MakeSubscriptionRequest(string topicName)
        {
            var request = new SubscribeRequest(topicName);
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, disposalTokenSource.Token);
        }

        public async void MakeUnsubscriptionRequest(string topicName)
        {
            //AddRequest(new UnsubscribeRequest(new NameIdPair(topicName, ID), topicName));
        }

        public async void MakeListTopicsRequest()
        {
            AddRequest(new ListTopicsRequest());
        }

        public async void MakePublishRequest<T>(T messageContent)
        {
            //var message = new Message<T>
            //{
            //    Timestamp = DateTime.UtcNow,
            //    PublisherInfo = new NameIdPair(Name, ID),
            //    Content = messageContent
            //};

            //AddRequest(new PublishRequest<T>(message));
        }

        //public Guid FindTopicID(string name)
        //{
        //    Guid found = Guid.Empty;
        //    int index = 0;
        //    while (found == Guid.Empty && index < TopicCache.Count)
        //    {
        //        if (TopicCache[index].Name == name)
        //            found = TopicCache[index].ID;
        //        index++;
        //    }
        //    return found;
        //}

        private static ArraySegment<byte> SerializeRequest(Request request)
        {
            var data = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            return buffer;
        }

        public void Dispose()
        {
            disposalTokenSource.Cancel();
            _ = WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
        }
    }
}
