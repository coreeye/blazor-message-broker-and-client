using System;

namespace MessageBroker.Shared
{
    public enum RequestType { SUBSCRIBE, UNSUBSCRIBE, PUBLISH, CREATE_TOPIC, LIST_TOPICS }

    [Serializable]
    public class Request : ModelBase
    {
    }

    [Serializable]
    public class SubscribeRequest : Request
    {
        public string TopicName { get; set; }

        public SubscribeRequest(string topicName)
        {
            TopicName = topicName;
        }
    }

    [Serializable]
    public class UnsubscribeRequest : Request
    {
        public string TopicName { get; set; }

        public UnsubscribeRequest(string topicName)
        {
            TopicName = topicName;
        }
    }

    [Serializable]
    public class PublishRequest : Request
    {
        public string Message { get; set; }

        public PublishRequest(string message)
        {
            Message = message;
        }
    }

    [Serializable]
    public class CreateTopicRequest : Request
    {
        public string TopicName { get; set; }

        public CreateTopicRequest(string name)
        {
            TopicName = name;
        }
    }

    [Serializable]
    public class ListTopicsRequest : Request
    {
        public ListTopicsRequest()
        {
        }
    }
}
