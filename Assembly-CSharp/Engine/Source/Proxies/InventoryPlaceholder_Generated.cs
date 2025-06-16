using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;

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
      return ServiceCache.Factory.Instantiate(this);
    }

    public void CopyTo(object target2)
    {
      InventoryPlaceholder_Generated placeholderGenerated = (InventoryPlaceholder_Generated) target2;
      placeholderGenerated.name = name;
      placeholderGenerated.grid = grid;
      placeholderGenerated.imageInventoryCell = imageInventoryCell;
      placeholderGenerated.imageInventorySlot = imageInventorySlot;
      placeholderGenerated.imageInventorySlotBig = imageInventorySlotBig;
      placeholderGenerated.imageInformation = imageInformation;
      placeholderGenerated.imageInformationSpecial = imageInformationSpecial;
      placeholderGenerated.soundGroup = soundGroup;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", id);
      UnityDataWriteUtility.Write(writer, "Grid", grid);
      UnityDataWriteUtility.Write(writer, "ImageInventoryCell", imageInventoryCell);
      UnityDataWriteUtility.Write(writer, "ImageInventorySlot", imageInventorySlot);
      UnityDataWriteUtility.Write(writer, "ImageInventorySlotBig", imageInventorySlotBig);
      UnityDataWriteUtility.Write(writer, "ImageInformation", imageInformation);
      UnityDataWriteUtility.Write(writer, "ImageInformationSpecial", imageInformationSpecial);
      UnityDataWriteUtility.Write<ItemSoundGroup>(writer, "SoundGroup", soundGroup);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      grid = UnityDataReadUtility.Read(reader, "Grid", grid);
      imageInventoryCell = UnityDataReadUtility.Read(reader, "ImageInventoryCell", imageInventoryCell);
      imageInventorySlot = UnityDataReadUtility.Read(reader, "ImageInventorySlot", imageInventorySlot);
      imageInventorySlotBig = UnityDataReadUtility.Read(reader, "ImageInventorySlotBig", imageInventorySlotBig);
      imageInformation = UnityDataReadUtility.Read(reader, "ImageInformation", imageInformation);
      imageInformationSpecial = UnityDataReadUtility.Read(reader, "ImageInformationSpecial", imageInformationSpecial);
      soundGroup = UnityDataReadUtility.Read<ItemSoundGroup>(reader, "SoundGroup", soundGroup);
    }
  }
}
