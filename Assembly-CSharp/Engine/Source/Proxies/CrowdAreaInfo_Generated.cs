using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Movable;
using Engine.Source.Components.Crowds;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CrowdAreaInfo))]
  public class CrowdAreaInfo_Generated : 
    CrowdAreaInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CrowdAreaInfo_Generated instance = Activator.CreateInstance<CrowdAreaInfo_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CrowdAreaInfo_Generated areaInfoGenerated = (CrowdAreaInfo_Generated) target2;
      areaInfoGenerated.Area = Area;
      CloneableObjectUtility.CopyListTo(areaInfoGenerated.TemplateInfos, TemplateInfos);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Area", Area);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Templates", TemplateInfos);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Area = DefaultDataReadUtility.ReadEnum<AreaEnum>(reader, "Area");
      TemplateInfos = DefaultDataReadUtility.ReadListSerialize(reader, "Templates", TemplateInfos);
    }
  }
}
