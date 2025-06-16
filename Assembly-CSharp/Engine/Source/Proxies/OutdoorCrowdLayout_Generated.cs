using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Crowds;
using Engine.Source.OutdoorCrowds;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdLayout))]
  public class OutdoorCrowdLayout_Generated : 
    OutdoorCrowdLayout,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdLayout_Generated instance = Activator.CreateInstance<OutdoorCrowdLayout_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdLayout_Generated crowdLayoutGenerated = (OutdoorCrowdLayout_Generated) target2;
      crowdLayoutGenerated.Layout = this.Layout;
      CloneableObjectUtility.CopyListTo<OutdoorCrowdState>(crowdLayoutGenerated.States, this.States);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<OutdoorCrowdLayoutEnum>(writer, "Layout", this.Layout);
      DefaultDataWriteUtility.WriteListSerialize<OutdoorCrowdState>(writer, "States", this.States);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Layout = DefaultDataReadUtility.ReadEnum<OutdoorCrowdLayoutEnum>(reader, "Layout");
      this.States = DefaultDataReadUtility.ReadListSerialize<OutdoorCrowdState>(reader, "States", this.States);
    }
  }
}
