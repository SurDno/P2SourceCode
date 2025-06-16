using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl;
using Engine.Source.Commons;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SceneObject))]
  public class SceneObject_Generated : 
    SceneObject,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone() => ServiceCache.Factory.Instantiate(this);

    public void CopyTo(object target2) => ((SceneObject_Generated) target2).name = name;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Items", Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      Items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", Items);
    }
  }
}
