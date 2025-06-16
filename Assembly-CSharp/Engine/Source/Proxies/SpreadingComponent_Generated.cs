using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((SpreadingComponent_Generated) target2).diseasedState = diseasedState;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "DiseasedState", diseasedState);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      diseasedState = DefaultDataReadUtility.ReadEnum<DiseasedStateEnum>(reader, "DiseasedState");
    }
  }
}
