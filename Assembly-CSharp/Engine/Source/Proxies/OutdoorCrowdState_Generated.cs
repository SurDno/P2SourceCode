using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdState))]
  public class OutdoorCrowdState_Generated : 
    OutdoorCrowdState,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdState_Generated instance = Activator.CreateInstance<OutdoorCrowdState_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdState_Generated crowdStateGenerated = (OutdoorCrowdState_Generated) target2;
      crowdStateGenerated.State = State;
      CloneableObjectUtility.CopyListTo(crowdStateGenerated.TemplateLinks, TemplateLinks);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "State", State);
      DefaultDataWriteUtility.WriteListSerialize(writer, "TemplateLinks", TemplateLinks);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      State = DefaultDataReadUtility.ReadEnum<DiseasedStateEnum>(reader, "State");
      TemplateLinks = DefaultDataReadUtility.ReadListSerialize(reader, "TemplateLinks", TemplateLinks);
    }
  }
}
