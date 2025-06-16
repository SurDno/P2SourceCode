using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Movable;
using Engine.Source.Components.Crowds;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CrowdAreaInfo_Generated areaInfoGenerated = (CrowdAreaInfo_Generated) target2;
      areaInfoGenerated.Area = this.Area;
      CloneableObjectUtility.CopyListTo<CrowdTemplateInfo>(areaInfoGenerated.TemplateInfos, this.TemplateInfos);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<AreaEnum>(writer, "Area", this.Area);
      DefaultDataWriteUtility.WriteListSerialize<CrowdTemplateInfo>(writer, "Templates", this.TemplateInfos);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Area = DefaultDataReadUtility.ReadEnum<AreaEnum>(reader, "Area");
      this.TemplateInfos = DefaultDataReadUtility.ReadListSerialize<CrowdTemplateInfo>(reader, "Templates", this.TemplateInfos);
    }
  }
}
