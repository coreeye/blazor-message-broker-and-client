using MessageBroker.Shared;

namespace MessageBroker.Server.Networking
{
    public interface IModelSerialization
    {
        TModel DeserializeModel<TModel>(string modelString)
               where TModel : ModelBase;
    }
}