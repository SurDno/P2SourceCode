// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.InventoryComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
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
