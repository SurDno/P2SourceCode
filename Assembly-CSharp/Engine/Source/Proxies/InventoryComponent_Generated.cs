using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((InventoryComponent_Generated) target2).containerResource = containerResource;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "ContainerResource", containerResource);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      containerResource = UnityDataReadUtility.Read(reader, "ContainerResource", containerResource);
    }
  }
}
