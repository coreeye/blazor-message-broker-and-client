using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MessageBroker.Shared
{
    public class Topic
    {
        public NameIdPair Info { get; set; }

        public HashSet<string> Subscribers { get; set; }

        public Topic(NameIdPair info)
        {
            Info = info;
            Subscribers = new HashSet<string>();
        }
    }
}
