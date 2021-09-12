
using Newtonsoft.Json;

namespace MessageBroker.Shared
{
    public static class ModelExtensions
    {
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
