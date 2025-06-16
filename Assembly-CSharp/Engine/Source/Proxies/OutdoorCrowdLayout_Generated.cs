using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Crowds;
using Engine.Source.OutdoorCrowds;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdLayout_Generated crowdLayoutGenerated = (OutdoorCrowdLayout_Generated) target2;
      crowdLayoutGenerated.Layout = Layout;
      CloneableObjectUtility.CopyListTo(crowdLayoutGenerated.States, States);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Layout", Layout);
      DefaultDataWriteUtility.WriteListSerialize(writer, "States", States);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Layout = DefaultDataReadUtility.ReadEnum<OutdoorCrowdLayoutEnum>(reader, "Layout");
      States = DefaultDataReadUtility.ReadListSerialize(reader, "States", States);
    }
  }
}
