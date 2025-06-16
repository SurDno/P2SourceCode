// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.InventoryPlaceholder_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryPlaceholder))]
  public class InventoryPlaceholder_Generated : 
    InventoryPlaceholder,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<InventoryPlaceholder_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      InventoryPlaceholder_Generated placeholderGenerated = (InventoryPlaceholder_Generated) target2;
      placeholderGenerated.name = this.name;
      placeholderGenerated.grid = this.grid;
      placeholderGenerated.imageInventoryCell = this.imageInventoryCell;
      placeholderGenerated.imageInventorySlot = this.imageInventorySlot;
      placeholderGenerated.imageInventorySlotBig = this.imageInventorySlotBig;
      placeholderGenerated.imageInformation = this.imageInformation;
      placeholderGenerated.imageInformationSpecial = this.imageInformationSpecial;
      placeholderGenerated.soundGroup = this.soundGroup;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      UnityDataWriteUtility.Write<IInventoryGridLimited>(writer, "Grid", this.grid);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageInventoryCell", this.imageInventoryCell);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageInventorySlot", this.imageInventorySlot);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageInventorySlotBig", this.imageInventorySlotBig);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageInformation", this.imageInformation);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageInformationSpecial", this.imageInformationSpecial);
      UnityDataWriteUtility.Write<ItemSoundGroup>(writer, "SoundGroup", this.soundGroup);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.grid = UnityDataReadUtility.Read<IInventoryGridLimited>(reader, "Grid", this.grid);
      this.imageInventoryCell = UnityDataReadUtility.Read<Sprite>(reader, "ImageInventoryCell", this.imageInventoryCell);
      this.imageInventorySlot = UnityDataReadUtility.Read<Sprite>(reader, "ImageInventorySlot", this.imageInventorySlot);
      this.imageInventorySlotBig = UnityDataReadUtility.Read<Sprite>(reader, "ImageInventorySlotBig", this.imageInventorySlotBig);
      this.imageInformation = UnityDataReadUtility.Read<Sprite>(reader, "ImageInformation", this.imageInformation);
      this.imageInformationSpecial = UnityDataReadUtility.Read<Sprite>(reader, "ImageInformationSpecial", this.imageInformationSpecial);
      this.soundGroup = UnityDataReadUtility.Read<ItemSoundGroup>(reader, "SoundGroup", this.soundGroup);
    }
  }
}
