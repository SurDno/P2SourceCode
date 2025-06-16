using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Assets.Internal.Reference;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl;
using Engine.Source.Commons;
using System;

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
    public object Clone() => (object) ServiceCache.Factory.Instantiate<SceneObject_Generated>(this);

    public void CopyTo(object target2) => ((EngineObject) target2).name = this.name;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<SceneObjectItem>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.Items = DefaultDataReadUtility.ReadListSerialize<SceneObjectItem>(reader, "Items", this.Items);
    }
  }
}
