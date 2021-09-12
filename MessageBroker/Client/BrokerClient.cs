using MessageBroker.Server.Networking;
using MessageBroker.Shared;
using Newtonsoft.Json;
using System;
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

        private CancellationTokenSource _disposalTokenSource = new CancellationTokenSource();

        private Action<string> _logCallback;

        private readonly IModelSerialization _serialization;

        public BrokerClient(IModelSerialization serialization) : base()
        {
            WebSocket = new ClientWebSocket();
            TopicCache = new List<NameIdPair>();
            _serialization = serialization;
        }

        public async void Connect(Uri uri, Action onConnectCallback = null, Action<string> logCallback = null)
        {
            try
            {
                _logCallback = logCallback;
                WebSocket = new ClientWebSocket();
                await WebSocket.ConnectAsync(uri, _disposalTokenSource.Token);
                onConnectCallback?.Invoke();
                _ = ReceiveLoop();
            }
            catch (Exception)
            {
                onConnectCallback?.Invoke();
                _logCallback?.Invoke("Could not connect.");
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            while (!_disposalTokenSource.IsCancellationRequested)
            {
                // Note that the received block might only be part of a larger message. If this applies in your scenario,
                // check the received.EndOfMessage and consider buffering the blocks until that property is true.
                // Or use a higher-level library such as SignalR.
                var received = await WebSocket.ReceiveAsync(buffer, _disposalTokenSource.Token);
                var message = Encoding.UTF8.GetString(buffer.Array, 0, received.Count);

                var response = _serialization.DeserializeModel<Response>(message);
                ProcessResponse(response);
            }
        }

        protected void ProcessResponse(Response response)
        {
            // Handle responses to previous client requests
            switch (response)
            {
                case NewMessageResponse newMessageResponse:
                    {
                        var logText = $"[New Message: \"{newMessageResponse.Message}\"";
                        _logCallback?.Invoke(logText);
                        break;
                    }

                case ListTopicsResponse listTopicsResponse:
                    {
                        // Cache the topic list and show the list of topics
                        TopicCache = listTopicsResponse.Topics.ToList();

                        _logCallback?.Invoke($"[Topic List]");
                        int index = 0;
                        foreach (NameIdPair topic in TopicCache)
                        {
                            _logCallback?.Invoke($" {index}. {topic.Name}");
                            index++;
                        }

                        break;
                    }

                case SubscribedResponse subscribedResponse:
                    {
                        // Add the new topic to the list of owned topics
                        var subscriptions = subscribedResponse.Subscriptions.Select(s => s.Name);

                        _logCallback?.Invoke($"Subscribe success");
                        subscriptions.ToList().ForEach(s => _logCallback?.Invoke($"[Current subscriptions \"{s}\" has been created]"));
                        break;
                    }

                case UnSubscribedResponse unsubscribedResponse:
                    {
                        // Add the new topic to the list of owned topics
                        var subscriptions = unsubscribedResponse.Subscriptions.Select(s => s.Name);

                        _logCallback?.Invoke($"Unsubscribe success");
                        subscriptions.ToList().ForEach(s => _logCallback?.Invoke($"[Current subscriptions \"{s}\" has been created]"));
                        break;
                    }

                case TopicCreatedResponse topicCreatedResponse:
                    {
                        // Add the new topic to the list of owned topics
                        var topicInfo = topicCreatedResponse.TopicInfo;
                        TopicCache.Add(topicInfo);

                        _logCallback?.Invoke($"[Topic \"{topicInfo.Name}\" has been created]");
                        break;
                    }

                case InfoResponse infoResponse:
                    _logCallback?.Invoke($"[Info] {infoResponse.Text}");
                    break;
            }
        }

        public async void MakeCreateTopicRequest(string topicName)
        {
            var request = new CreateTopicRequest(topicName);
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _disposalTokenSource.Token);
        }

        public async void MakeSubscriptionRequest(string topicName)
        {
            var request = new SubscribeRequest(topicName);
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _disposalTokenSource.Token);
        }

        public async void MakeUnsubscriptionRequest(string topicName)
        {
            var request = new UnsubscribeRequest(topicName);
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _disposalTokenSource.Token);
        }

        public async void MakeListTopicsRequest()
        {
            var request = new ListTopicsRequest();
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _disposalTokenSource.Token);
        }

        public async void MakePublishRequest(string messageContent)
        {
            var request = new PublishRequest(messageContent);
            var buffer = SerializeRequest(request);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _disposalTokenSource.Token);
        }

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
            _disposalTokenSource.Cancel();
            _ = WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
        }
    }
}
