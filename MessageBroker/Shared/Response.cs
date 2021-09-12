using System;
using System.Collections.Generic;

namespace MessageBroker.Shared
{
    [Serializable]
    public class Response : ModelBase
    {
    }

    [Serializable]
    public class InfoResponse : Response
    {
        public string Text { get; set; }

        public InfoResponse(string text)
        {
            Text = text;
        }
    }

    [Serializable]
    public class SubscribedResponse : Response
    {
        public NameIdPair[] Subscriptions { get; set; }

        public SubscribedResponse(NameIdPair[] subscriptions)
        {
            Subscriptions = subscriptions;
        }
    }

    [Serializable]
    public class UnSubscribedResponse : Response
    {
        public NameIdPair[] Subscriptions { get; set; }

        public UnSubscribedResponse(NameIdPair[] subscriptions)
        {
            Subscriptions = subscriptions;
        }
    }

    [Serializable]
    public class NewMessageResponse : Response
    {
        public string Message { get; set; }

        public NewMessageResponse(string message)
        {
            Message = message;
        }
    }

    [Serializable]
    public class TopicCreatedResponse : Response
    {
        public NameIdPair TopicInfo { get; set; }

        public TopicCreatedResponse(NameIdPair topicInfo)
        {
            TopicInfo = topicInfo;
        }
    }

    [Serializable]
    public class ListTopicsResponse : Response
    {
        public NameIdPair[] Topics { get; set; }

        public ListTopicsResponse(NameIdPair[] topicList)
        {
            Topics = topicList;
        }
    }
}
