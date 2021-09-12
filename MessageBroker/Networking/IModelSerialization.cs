using MessageBroker.Shared;

namespace MessageBroker.Server.Networking
{
    public interface IModelSerialization
    {
        TModel DeserializeModel<TModel>(byte[] bytes, int count)
               where TModel : ModelBase;
    }
}