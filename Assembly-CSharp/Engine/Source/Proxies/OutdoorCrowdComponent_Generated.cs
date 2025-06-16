using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Crowds;
using Engine.Source.Components;
using Engine.Source.OutdoorCrowds;
using Scripts.Tools.Serializations.Converters;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((OutdoorCrowdComponent) target2).data = this.data;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<OutdoorCrowdData>(writer, "Data", this.data);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.data = UnityDataReadUtility.Read<OutdoorCrowdData>(reader, "Data", this.data);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<OutdoorCrowdLayoutEnum>(writer, "Layout", this.layout);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.layout = DefaultDataReadUtility.ReadEnum<OutdoorCrowdLayoutEnum>(reader, "Layout");
    }
  }
}
