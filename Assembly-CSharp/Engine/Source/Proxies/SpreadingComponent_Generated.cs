using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SpreadingComponent))]
  public class SpreadingComponent_Generated : 
    SpreadingComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SpreadingComponent_Generated instance = Activator.CreateInstance<SpreadingComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((SpreadingComponent) target2).diseasedState = this.diseasedState;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<DiseasedStateEnum>(writer, "DiseasedState", this.diseasedState);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.diseasedState = DefaultDataReadUtility.ReadEnum<DiseasedStateEnum>(reader, "DiseasedState");
    }
  }
}
