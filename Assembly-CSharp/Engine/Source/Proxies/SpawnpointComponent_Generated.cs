using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Milestone;
using Engine.Source.Components;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((SpawnpointComponent) target2).type = this.type;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<Kind>(writer, "Type", this.type);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.type = DefaultDataReadUtility.ReadEnum<Kind>(reader, "Type");
    }
  }
}
