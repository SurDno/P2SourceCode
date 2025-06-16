using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Source.OutdoorCrowds;

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
      return ServiceCache.Factory.Instantiate(this);
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdData_Generated crowdDataGenerated = (OutdoorCrowdData_Generated) target2;
      crowdDataGenerated.name = name;
      crowdDataGenerated.TableName = TableName;
      crowdDataGenerated.Region = Region;
      CloneableObjectUtility.CopyListTo(crowdDataGenerated.Layouts, Layouts);
      CloneableObjectUtility.CopyListTo(crowdDataGenerated.Templates, Templates);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "TableName", TableName);
      DefaultDataWriteUtility.WriteEnum(writer, "Region", Region);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Layouts", Layouts);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Templates", Templates);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      TableName = DefaultDataReadUtility.Read(reader, "TableName", TableName);
      Region = DefaultDataReadUtility.ReadEnum<RegionEnum>(reader, "Region");
      Layouts = DefaultDataReadUtility.ReadListSerialize(reader, "Layouts", Layouts);
      Templates = DefaultDataReadUtility.ReadListSerialize(reader, "Templates", Templates);
    }
  }
}
