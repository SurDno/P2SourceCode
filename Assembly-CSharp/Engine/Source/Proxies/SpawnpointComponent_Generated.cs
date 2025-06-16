using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Milestone;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SpawnpointComponent))]
  public class SpawnpointComponent_Generated : 
    SpawnpointComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SpawnpointComponent_Generated instance = Activator.CreateInstance<SpawnpointComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((SpawnpointComponent_Generated) target2).type = type;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Type", type);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.type = DefaultDataReadUtility.ReadEnum<Kind>(reader, "Type");
    }
  }
}
