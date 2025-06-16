using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Crowds;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdComponent))]
  public class OutdoorCrowdComponent_Generated : 
    OutdoorCrowdComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      OutdoorCrowdComponent_Generated instance = Activator.CreateInstance<OutdoorCrowdComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((OutdoorCrowdComponent_Generated) target2).data = data;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Data", data);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      data = UnityDataReadUtility.Read(reader, "Data", data);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Layout", layout);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      layout = DefaultDataReadUtility.ReadEnum<OutdoorCrowdLayoutEnum>(reader, "Layout");
    }
  }
}
