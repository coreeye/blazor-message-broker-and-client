using MessageBroker.Shared;
using Newtonsoft.Json;
using System;
using System.Text;

namespace MessageBroker.Server.Networking
{
    public class ModelSerialization : IModelSerialization
    {
        public ArraySegment<byte> SerializeModel(ModelBase model)
        {
            var data = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            return buffer;
        }

        public TModel DeserializeModel<TModel>(byte[] bytes, int count)
            where TModel : ModelBase
        {
            var message = Encoding.UTF8.GetString(bytes, 0, count);
            return JsonConvert.DeserializeObject<TModel>(message, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
