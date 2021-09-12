using System;

namespace MessageBroker.Shared
{
    [Serializable]
    public class Message<T>
    {
        public DateTime Timestamp { get; set; }

        public T Content { get; set; }
    }
}
