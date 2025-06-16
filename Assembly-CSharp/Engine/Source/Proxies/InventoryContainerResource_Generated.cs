// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.InventoryContainerResource_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Storable;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryContainerResource))]
  public class InventoryContainerResource_Generated : 
    InventoryContainerResource,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      InventoryContainerResource_Generated instance = Activator.CreateInstance<InventoryContainerResource_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      InventoryContainerResource_Generated resourceGenerated = (InventoryContainerResource_Generated) target2;
      resourceGenerated.name = this.name;
      resourceGenerated.kind = this.kind;
      resourceGenerated.slotKind = this.slotKind;
      resourceGenerated.grid = this.grid;
      resourceGenerated.group = this.group;
      resourceGenerated.instrument = this.instrument;
      CloneableObjectUtility.CopyListTo<InventoryContainerOpenResource>(resourceGenerated.openResources, this.openResources);
      resourceGenerated.difficulty = this.difficulty;
      resourceGenerated.instrumentDamage = this.instrumentDamage;
      resourceGenerated.position = this.position;
      resourceGenerated.anchor = this.anchor;
      resourceGenerated.pivot = this.pivot;
      resourceGenerated.imageBackground = this.imageBackground;
      resourceGenerated.imageInstrument = this.imageInstrument;
      resourceGenerated.imageLock = this.imageLock;
      resourceGenerated.imageNotAvailable = this.imageNotAvailable;
      CloneableObjectUtility.FillListTo<StorableGroup>(resourceGenerated.limitations, this.limitations);
      CloneableObjectUtility.FillListTo<StorableGroup>(resourceGenerated.except, this.except);
      resourceGenerated.openTime = this.openTime;
      resourceGenerated.imageForeground = this.imageForeground;
      resourceGenerated.openStartAudio = this.openStartAudio;
      resourceGenerated.openProgressAudio = this.openProgressAudio;
      resourceGenerated.openCompleteAudio = this.openCompleteAudio;
      resourceGenerated.openCancelAudio = this.openCancelAudio;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteEnum<ContainerCellKind>(writer, "Kind", this.kind);
      DefaultDataWriteUtility.WriteEnum<SlotKind>(writer, "SlotKind", this.slotKind);
      UnityDataWriteUtility.Write<IInventoryGridBase>(writer, "Grid", this.grid);
      DefaultDataWriteUtility.WriteEnum<InventoryGroup>(writer, "Group", this.group);
      DefaultDataWriteUtility.WriteEnum<StorableGroup>(writer, "Instrument", this.instrument);
      DefaultDataWriteUtility.WriteListSerialize<InventoryContainerOpenResource>(writer, "OpenResources", this.openResources);
      DefaultDataWriteUtility.Write(writer, "Difficulty", this.difficulty);
      DefaultDataWriteUtility.Write(writer, "InstrumentDamage", this.instrumentDamage);
      EngineDataWriteUtility.Write(writer, "Position", this.position);
      EngineDataWriteUtility.Write(writer, "Anchor", this.anchor);
      EngineDataWriteUtility.Write(writer, "Pivot", this.pivot);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageBackground", this.imageBackground);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageInstrument", this.imageInstrument);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageLock", this.imageLock);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageNotAvailable", this.imageNotAvailable);
      DefaultDataWriteUtility.WriteListEnum<StorableGroup>(writer, "Limitations", this.limitations);
      DefaultDataWriteUtility.WriteListEnum<StorableGroup>(writer, "Except", this.except);
      DefaultDataWriteUtility.Write(writer, "OpenTime", this.openTime);
      UnityDataWriteUtility.Write<Sprite>(writer, "ImageForeground", this.imageForeground);
      UnityDataWriteUtility.Write<AudioClip>(writer, "OpenStartAudio", this.openStartAudio);
      UnityDataWriteUtility.Write<AudioClip>(writer, "OpenProgressAudio", this.openProgressAudio);
      UnityDataWriteUtility.Write<AudioClip>(writer, "OpenCompleteAudio", this.openCompleteAudio);
      UnityDataWriteUtility.Write<AudioClip>(writer, "OpenCancelAudio", this.openCancelAudio);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.kind = DefaultDataReadUtility.ReadEnum<ContainerCellKind>(reader, "Kind");
      this.slotKind = DefaultDataReadUtility.ReadEnum<SlotKind>(reader, "SlotKind");
      this.grid = UnityDataReadUtility.Read<IInventoryGridBase>(reader, "Grid", this.grid);
      this.group = DefaultDataReadUtility.ReadEnum<InventoryGroup>(reader, "Group");
      this.instrument = DefaultDataReadUtility.ReadEnum<StorableGroup>(reader, "Instrument");
      this.openResources = DefaultDataReadUtility.ReadListSerialize<InventoryContainerOpenResource>(reader, "OpenResources", this.openResources);
      this.difficulty = DefaultDataReadUtility.Read(reader, "Difficulty", this.difficulty);
      this.instrumentDamage = DefaultDataReadUtility.Read(reader, "InstrumentDamage", this.instrumentDamage);
      this.position = EngineDataReadUtility.Read(reader, "Position", this.position);
      this.anchor = EngineDataReadUtility.Read(reader, "Anchor", this.anchor);
      this.pivot = EngineDataReadUtility.Read(reader, "Pivot", this.pivot);
      this.imageBackground = UnityDataReadUtility.Read<Sprite>(reader, "ImageBackground", this.imageBackground);
      this.imageInstrument = UnityDataReadUtility.Read<Sprite>(reader, "ImageInstrument", this.imageInstrument);
      this.imageLock = UnityDataReadUtility.Read<Sprite>(reader, "ImageLock", this.imageLock);
      this.imageNotAvailable = UnityDataReadUtility.Read<Sprite>(reader, "ImageNotAvailable", this.imageNotAvailable);
      this.limitations = DefaultDataReadUtility.ReadListEnum<StorableGroup>(reader, "Limitations", this.limitations);
      this.except = DefaultDataReadUtility.ReadListEnum<StorableGroup>(reader, "Except", this.except);
      this.openTime = DefaultDataReadUtility.Read(reader, "OpenTime", this.openTime);
      this.imageForeground = UnityDataReadUtility.Read<Sprite>(reader, "ImageForeground", this.imageForeground);
      this.openStartAudio = UnityDataReadUtility.Read<AudioClip>(reader, "OpenStartAudio", this.openStartAudio);
      this.openProgressAudio = UnityDataReadUtility.Read<AudioClip>(reader, "OpenProgressAudio", this.openProgressAudio);
      this.openCompleteAudio = UnityDataReadUtility.Read<AudioClip>(reader, "OpenCompleteAudio", this.openCompleteAudio);
      this.openCancelAudio = UnityDataReadUtility.Read<AudioClip>(reader, "OpenCancelAudio", this.openCancelAudio);
    }
  }
}
