using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Inventory;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryGridInfinited))]
  public class InventoryGridInfinited_Generated : 
    InventoryGridInfinited,
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
      InventoryGridInfinited_Generated infinitedGenerated = (InventoryGridInfinited_Generated) target2;
      infinitedGenerated.name = name;
      infinitedGenerated.columns = columns;
      infinitedGenerated.rows = rows;
      infinitedGenerated.direction = direction;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "Columns", columns);
      DefaultDataWriteUtility.Write(writer, "Rows", rows);
      DefaultDataWriteUtility.WriteEnum(writer, "Direction", direction);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      columns = DefaultDataReadUtility.Read(reader, "Columns", columns);
      rows = DefaultDataReadUtility.Read(reader, "Rows", rows);
      direction = DefaultDataReadUtility.ReadEnum<DirectionKind>(reader, "Direction");
    }
  }
}
