using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Inventory;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryGridLimited))]
  public class InventoryGridLimited_Generated : 
    InventoryGridLimited,
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
      InventoryGridLimited_Generated limitedGenerated = (InventoryGridLimited_Generated) target2;
      limitedGenerated.name = name;
      CloneableObjectUtility.CopyListTo(limitedGenerated.cells, cells);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Cells", cells);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      cells = DefaultDataReadUtility.ReadListSerialize(reader, "Cells", cells);
    }
  }
}
