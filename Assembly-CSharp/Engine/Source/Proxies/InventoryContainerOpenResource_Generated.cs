using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryContainerOpenResource))]
  public class InventoryContainerOpenResource_Generated : 
    InventoryContainerOpenResource,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      InventoryContainerOpenResource_Generated instance = Activator.CreateInstance<InventoryContainerOpenResource_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      InventoryContainerOpenResource_Generated resourceGenerated = (InventoryContainerOpenResource_Generated) target2;
      resourceGenerated.resource = this.resource;
      resourceGenerated.amount = this.amount;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<IEntity>(writer, "Resource", this.resource);
      DefaultDataWriteUtility.Write(writer, "Amount", this.amount);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.resource = UnityDataReadUtility.Read<IEntity>(reader, "Resource", this.resource);
      this.amount = DefaultDataReadUtility.Read(reader, "Amount", this.amount);
    }
  }
}
