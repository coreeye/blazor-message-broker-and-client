using System;
using System.Collections.Generic;

namespace MessageBroker.Shared
{
    public enum ResponseType { INFO, NEW_MESSAGE, TOPIC_CREATED, LIST_TOPICS, SUBSCRIBED, UNSUBSCRIBED }

    [Serializable]
    public class Response
    {
        public ResponseType Type { get; set; }
    }

    [Serializable]
    public class InfoResponse : Response
    {
        public string Text { get; set; }

        public InfoResponse(string text)
        {
            Type = ResponseType.INFO;
            Text = text;
        }
    }

    [Serializable]
    public class SubscribedResponse : Response
    {
        public List<NameIdPair> Subscriptions { get; set; }

        public SubscribedResponse(List<NameIdPair> subscriptions)
        {
            Type = ResponseType.SUBSCRIBED;
            Subscriptions = subscriptions;
        }
    }

    [Serializable]
    public class UnSubscribedResponse : Response
    {
        public List<NameIdPair> Subscriptions { get; set; }

        public UnSubscribedResponse(List<NameIdPair> subscriptions)
        {
            Type = ResponseType.UNSUBSCRIBED;
            Subscriptions = subscriptions;
        }
    }

    [Serializable]
    public class NewMessageResponse<T> : Response
    {
        public Message<T> Message { get; set; }

        public NewMessageResponse(Message<T> message)
        {
            Type = ResponseType.NEW_MESSAGE;
            Message = message;
        }
    }

    [Serializable]
    public class TopicCreatedResponse : Response
    {
        public NameIdPair TopicInfo { get; set; }

        public TopicCreatedResponse(NameIdPair topicInfo)
        {
            Type = ResponseType.TOPIC_CREATED;
            TopicInfo = topicInfo;
        }
    }

    [Serializable]
    public class ListTopicsResponse : Response
    {
        public List<NameIdPair> Topics { get; set; }

        public ListTopicsResponse(List<NameIdPair> topicList)
        {
            Type = ResponseType.LIST_TOPICS;
            Topics = topicList;
        }
    }
}
