using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdTemplateLink_Generated templateLinkGenerated = (OutdoorCrowdTemplateLink_Generated) target2;
      templateLinkGenerated.Link = Link;
      CloneableObjectUtility.FillListTo(templateLinkGenerated.Areas, Areas);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Link", Link);
      DefaultDataWriteUtility.WriteListEnum(writer, "Areas", Areas);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Link = DefaultDataReadUtility.Read(reader, "Link", Link);
      Areas = DefaultDataReadUtility.ReadListEnum(reader, "Areas", Areas);
    }
  }
}
