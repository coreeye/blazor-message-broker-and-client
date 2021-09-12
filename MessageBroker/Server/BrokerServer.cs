using MessageBroker.Server.Networking;
using MessageBroker.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker.Server
{
    /// <summary>
    /// Specialized server class to act as a message broker in a publish-subscribe system
    /// </summary>
    public class BrokerServer : WebSocketHandler, IBrokerServer
    {
        protected ConcurrentDictionary<Guid, Topic> _topics;

        protected ConcurrentDictionary<Guid, ConcurrentQueue<Response>> _responseQueues;

        public BrokerServer(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            _topics = new ConcurrentDictionary<Guid, Topic>();
            _responseQueues = new ConcurrentDictionary<Guid, ConcurrentQueue<Response>>();
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var request = message.DeserializeModel<Request>();

            var response = ProcessRequest(request, socketId);
            await SendMessageAsync(socket, response);
        }

        protected Response ProcessRequest(Request request, string socketId)
        {
            Response response = new InfoResponse("Error: Unknown request");

            switch (request)
            {
                case CreateTopicRequest createTopicRequest:
                    {
                        string topicName = createTopicRequest.TopicName;
                        if(string.IsNullOrEmpty(topicName))
                        {
                            response = new InfoResponse("Topic name cannot be null or empty");
                        }
                        else if(_topics.FirstOrDefault(s => s.Value.Info.Name.Equals(topicName))
                                .Equals(default(KeyValuePair<Guid, Topic>)) == false)
                        {
                            response = new InfoResponse("A topic with the same name exists already");
                        }
                        else
                        {
                            var topicInfo = CreateTopic(topicName);
                            Console.WriteLine($"Creating topic: {topicInfo.Name}");
                            response = new TopicCreatedResponse(topicInfo);
                        }
                        break;
                    }

                case ListTopicsRequest listTopicsRequest:
                    response = new ListTopicsResponse(GetClientTopicList().ToArray());
                    break;
                case PublishRequest publishRequest:
                    {
                        var message = publishRequest.Message;

                        if (string.IsNullOrEmpty(message) == false
                            && PublishMessage(message, socketId))
                        {
                            response = new InfoResponse("Message published to topics");
                        }
                        else
                            response = new InfoResponse("Message could not be published");
                        break;
                    }

                case SubscribeRequest subscribeRequest:
                    Console.WriteLine($"{socketId} wants to subscribe to \"{subscribeRequest.TopicName}\"");

                    if (Subscribe(subscribeRequest, socketId))
                    {
                        var topicsInfo = GetAllSubscriptions(socketId).Select(s => s.Info).ToArray();
                        response = new SubscribedResponse(topicsInfo);
                    }
                    else
                        response = new InfoResponse("Unable to add subscription (Topic does not exist)");
                    break;
                case UnsubscribeRequest unsubscribeRequest:
                    Console.WriteLine($"{socketId} wants to unsubscribe from \"{unsubscribeRequest.TopicName}\"");

                    if (Unsubscribe(unsubscribeRequest, socketId))
                    {
                        var topicsInfo = GetAllSubscriptions(socketId).Select(s => s.Info).ToArray();
                        response = new UnSubscribedResponse(topicsInfo);
                    }
                    else
                        response = new InfoResponse("Unable to remove subscription (Subscription may not exist)");
                    break;
            }

            return response;
        }

        protected NameIdPair CreateTopic(string name)
        {
            var id = Guid.NewGuid();
            var topicInfo = new NameIdPair(name, id);
            _topics[id] = new Topic(topicInfo);
            return topicInfo;
        }

        protected bool PublishMessage(string message, string socketId)
        {
            var topics = GetAllSubscriptions(socketId);

            foreach (var topic in topics)
            {
                foreach (string subscriberId in topic.Subscribers)
                {
                    SendMessageAsync(subscriberId, new NewMessageResponse(message));
                }

                return true;
            }

            return false;
        }

        protected bool Subscribe(SubscribeRequest request, string socketId)
        {
            var topic = _topics.FirstOrDefault(s => s.Value.Info.Name.Equals(request.TopicName));

            if (topic.Equals(default(KeyValuePair<Guid, Topic>)) == false)
            {
                topic.Value.Subscribers.Add(socketId);
                return true;
            }

            return false;
        }

        protected bool Unsubscribe(UnsubscribeRequest request, string socketId)
        {
            var topic = _topics.FirstOrDefault(s => s.Value.Info.Name.Equals(request.TopicName));

            if (topic.Equals(default(KeyValuePair<Guid, Topic>)) == false
                && topic.Value.Subscribers.Contains(socketId))
            {
                topic.Value.Subscribers.Remove(socketId);
                return true;
            }

            return false;
        }

        protected List<NameIdPair> GetClientTopicList()
        {
            var clientTopicList = new List<NameIdPair>();
            foreach (var topic in _topics.Values)
                clientTopicList.Add(topic.Info);
            return clientTopicList;
        }

        protected IEnumerable<Topic> GetAllSubscriptions(string subscriberID)
        {
            return _topics.Where(s => s.Value.Subscribers.Contains(subscriberID))
                .Select(s => s.Value);
        }
    }
}
