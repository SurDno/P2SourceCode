using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Inventory;
using System;

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
      return (object) ServiceCache.Factory.Instantiate<InventoryGridLimited_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      InventoryGridLimited_Generated limitedGenerated = (InventoryGridLimited_Generated) target2;
      limitedGenerated.name = this.name;
      CloneableObjectUtility.CopyListTo<Cell>(limitedGenerated.cells, this.cells);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<Cell>(writer, "Cells", this.cells);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.cells = DefaultDataReadUtility.ReadListSerialize<Cell>(reader, "Cells", this.cells);
    }
  }
}
