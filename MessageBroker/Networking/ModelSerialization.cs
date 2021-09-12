using MessageBroker.Shared;
using Newtonsoft.Json;

namespace MessageBroker.Server.Networking
{
    public class ModelSerialization : IModelSerialization
    {
        public TModel DeserializeModel<TModel>(string modelString)
            where TModel : ModelBase
        {
            return JsonConvert.DeserializeObject<TModel>(modelString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
