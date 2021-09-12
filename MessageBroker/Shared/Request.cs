using System;

namespace MessageBroker.Shared
{
    public enum RequestType { SUBSCRIBE, UNSUBSCRIBE, PUBLISH, CREATE_TOPIC, LIST_TOPICS }

    [Serializable]
    public class Request
    {
        public RequestType Type { get; set; }
    }

    [Serializable]
    public class SubscribeRequest : Request
    {
        public string TopicName { get; set; }

        public SubscribeRequest(string topicName)
        {
            Type = RequestType.SUBSCRIBE;
            TopicName = topicName;
        }
    }

    [Serializable]
    public class UnsubscribeRequest : Request
    {
        public string TopicName { get; set; }

        public UnsubscribeRequest(string topicName)
        {
            Type = RequestType.UNSUBSCRIBE;
            TopicName = topicName;
        }
    }

    [Serializable]
    public class PublishRequest<T> : Request
    {
        public Message<T> Message { get; set; }

        public PublishRequest(Message<T> message)
        {
            Type = RequestType.PUBLISH;
            Message = message;
        }
    }

    [Serializable]
    public class CreateTopicRequest : Request
    {
        public string TopicName { get; set; }

        public CreateTopicRequest(string name)
        {
            Type = RequestType.CREATE_TOPIC;
            TopicName = name;
        }
    }

    [Serializable]
    public class ListTopicsRequest : Request
    {
        public ListTopicsRequest()
        {
            Type = RequestType.LIST_TOPICS;
        }
    }
}
