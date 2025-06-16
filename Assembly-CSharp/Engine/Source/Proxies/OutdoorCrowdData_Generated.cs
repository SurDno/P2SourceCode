using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Source.OutdoorCrowds;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdData))]
  public class OutdoorCrowdData_Generated : 
    OutdoorCrowdData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<OutdoorCrowdData_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdData_Generated crowdDataGenerated = (OutdoorCrowdData_Generated) target2;
      crowdDataGenerated.name = this.name;
      crowdDataGenerated.TableName = this.TableName;
      crowdDataGenerated.Region = this.Region;
      CloneableObjectUtility.CopyListTo<OutdoorCrowdLayout>(crowdDataGenerated.Layouts, this.Layouts);
      CloneableObjectUtility.CopyListTo<OutdoorCrowdTemplates>(crowdDataGenerated.Templates, this.Templates);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "TableName", this.TableName);
      DefaultDataWriteUtility.WriteEnum<RegionEnum>(writer, "Region", this.Region);
      DefaultDataWriteUtility.WriteListSerialize<OutdoorCrowdLayout>(writer, "Layouts", this.Layouts);
      DefaultDataWriteUtility.WriteListSerialize<OutdoorCrowdTemplates>(writer, "Templates", this.Templates);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.TableName = DefaultDataReadUtility.Read(reader, "TableName", this.TableName);
      this.Region = DefaultDataReadUtility.ReadEnum<RegionEnum>(reader, "Region");
      this.Layouts = DefaultDataReadUtility.ReadListSerialize<OutdoorCrowdLayout>(reader, "Layouts", this.Layouts);
      this.Templates = DefaultDataReadUtility.ReadListSerialize<OutdoorCrowdTemplates>(reader, "Templates", this.Templates);
    }
  }
}
