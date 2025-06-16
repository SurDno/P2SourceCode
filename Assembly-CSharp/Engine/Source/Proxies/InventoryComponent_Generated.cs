using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryComponent))]
  public class InventoryComponent_Generated : 
    InventoryComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      InventoryComponent_Generated instance = Activator.CreateInstance<InventoryComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((InventoryComponent) target2).containerResource = this.containerResource;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<InventoryContainerResource>(writer, "ContainerResource", this.containerResource);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.containerResource = UnityDataReadUtility.Read<InventoryContainerResource>(reader, "ContainerResource", this.containerResource);
    }
  }
}
