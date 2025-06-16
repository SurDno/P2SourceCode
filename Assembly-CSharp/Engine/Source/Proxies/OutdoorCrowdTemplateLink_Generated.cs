using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Movable;
using Engine.Source.OutdoorCrowds;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdTemplateLink))]
  public class OutdoorCrowdTemplateLink_Generated : 
    OutdoorCrowdTemplateLink,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdTemplateLink_Generated instance = Activator.CreateInstance<OutdoorCrowdTemplateLink_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdTemplateLink_Generated templateLinkGenerated = (OutdoorCrowdTemplateLink_Generated) target2;
      templateLinkGenerated.Link = this.Link;
      CloneableObjectUtility.FillListTo<AreaEnum>(templateLinkGenerated.Areas, this.Areas);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Link", this.Link);
      DefaultDataWriteUtility.WriteListEnum<AreaEnum>(writer, "Areas", this.Areas);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Link = DefaultDataReadUtility.Read(reader, "Link", this.Link);
      this.Areas = DefaultDataReadUtility.ReadListEnum<AreaEnum>(reader, "Areas", this.Areas);
    }
  }
}
