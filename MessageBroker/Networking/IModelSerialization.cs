using MessageBroker.Shared;
using System;

namespace MessageBroker.Server.Networking
{
    public interface IModelSerialization
    {
        TModel DeserializeModel<TModel>(byte[] bytes, int count)
               where TModel : ModelBase;

        ArraySegment<byte> SerializeModel(ModelBase model);
    }
}