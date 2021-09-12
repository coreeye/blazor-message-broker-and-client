using System;

namespace MessageBroker.Shared
{
    [Serializable]
    public class NameIdPair
    {
        public string Name { get; set; }

        public Guid ID { get; set; }

        public NameIdPair(string name, Guid id)
        {
            Name = name;
            ID = id;
        }
    }
}
