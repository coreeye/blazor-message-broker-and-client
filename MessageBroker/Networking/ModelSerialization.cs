using MessageBroker.Shared;
using Newtonsoft.Json;
using System.Text;

namespace MessageBroker.Server.Networking
{
    public class ModelSerialization : IModelSerialization
    {
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
