
using Newtonsoft.Json;
using System;
using System.Text;

namespace MessageBroker.Shared
{
    public static class ModelExtensions
    {
        public static ArraySegment<byte> SerializeModel(this ModelBase modelBase)
        {
            var data = JsonConvert.SerializeObject(modelBase, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            return buffer;
        }

        public static TModel DeserializeModel<TModel>(this string modelString)
            where TModel : ModelBase
        {
            return JsonConvert.DeserializeObject<TModel>(modelString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
