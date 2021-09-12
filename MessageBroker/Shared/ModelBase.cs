using Newtonsoft.Json;
using System;
using System.Text;

namespace MessageBroker.Shared
{
    public abstract class ModelBase
    {
        public virtual ArraySegment<byte> SerializeModel()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            return buffer;
        }
    }
}