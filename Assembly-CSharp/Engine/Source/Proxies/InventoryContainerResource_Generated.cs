using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Storable;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(InventoryContainerResource))]
public class InventoryContainerResource_Generated :
	InventoryContainerResource,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<InventoryContainerResource_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var resourceGenerated = (InventoryContainerResource_Generated)target2;
		resourceGenerated.name = name;
		resourceGenerated.kind = kind;
		resourceGenerated.slotKind = slotKind;
		resourceGenerated.grid = grid;
		resourceGenerated.group = group;
		resourceGenerated.instrument = instrument;
		CloneableObjectUtility.CopyListTo(resourceGenerated.openResources, openResources);
		resourceGenerated.difficulty = difficulty;
		resourceGenerated.instrumentDamage = instrumentDamage;
		resourceGenerated.position = position;
		resourceGenerated.anchor = anchor;
		resourceGenerated.pivot = pivot;
		resourceGenerated.imageBackground = imageBackground;
		resourceGenerated.imageInstrument = imageInstrument;
		resourceGenerated.imageLock = imageLock;
		resourceGenerated.imageNotAvailable = imageNotAvailable;
		CloneableObjectUtility.FillListTo(resourceGenerated.limitations, limitations);
		CloneableObjectUtility.FillListTo(resourceGenerated.except, except);
		resourceGenerated.openTime = openTime;
		resourceGenerated.imageForeground = imageForeground;
		resourceGenerated.openStartAudio = openStartAudio;
		resourceGenerated.openProgressAudio = openProgressAudio;
		resourceGenerated.openCompleteAudio = openCompleteAudio;
		resourceGenerated.openCancelAudio = openCancelAudio;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteEnum(writer, "Kind", kind);
		DefaultDataWriteUtility.WriteEnum(writer, "SlotKind", slotKind);
		UnityDataWriteUtility.Write(writer, "Grid", grid);
		DefaultDataWriteUtility.WriteEnum(writer, "Group", group);
		DefaultDataWriteUtility.WriteEnum(writer, "Instrument", instrument);
		DefaultDataWriteUtility.WriteListSerialize(writer, "OpenResources", openResources);
		DefaultDataWriteUtility.Write(writer, "Difficulty", difficulty);
		DefaultDataWriteUtility.Write(writer, "InstrumentDamage", instrumentDamage);
		EngineDataWriteUtility.Write(writer, "Position", position);
		EngineDataWriteUtility.Write(writer, "Anchor", anchor);
		EngineDataWriteUtility.Write(writer, "Pivot", pivot);
		UnityDataWriteUtility.Write(writer, "ImageBackground", imageBackground);
		UnityDataWriteUtility.Write(writer, "ImageInstrument", imageInstrument);
		UnityDataWriteUtility.Write(writer, "ImageLock", imageLock);
		UnityDataWriteUtility.Write(writer, "ImageNotAvailable", imageNotAvailable);
		DefaultDataWriteUtility.WriteListEnum(writer, "Limitations", limitations);
		DefaultDataWriteUtility.WriteListEnum(writer, "Except", except);
		DefaultDataWriteUtility.Write(writer, "OpenTime", openTime);
		UnityDataWriteUtility.Write(writer, "ImageForeground", imageForeground);
		UnityDataWriteUtility.Write(writer, "OpenStartAudio", openStartAudio);
		UnityDataWriteUtility.Write(writer, "OpenProgressAudio", openProgressAudio);
		UnityDataWriteUtility.Write(writer, "OpenCompleteAudio", openCompleteAudio);
		UnityDataWriteUtility.Write(writer, "OpenCancelAudio", openCancelAudio);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		kind = DefaultDataReadUtility.ReadEnum<ContainerCellKind>(reader, "Kind");
		slotKind = DefaultDataReadUtility.ReadEnum<SlotKind>(reader, "SlotKind");
		grid = UnityDataReadUtility.Read(reader, "Grid", grid);
		group = DefaultDataReadUtility.ReadEnum<InventoryGroup>(reader, "Group");
		instrument = DefaultDataReadUtility.ReadEnum<StorableGroup>(reader, "Instrument");
		openResources = DefaultDataReadUtility.ReadListSerialize(reader, "OpenResources", openResources);
		difficulty = DefaultDataReadUtility.Read(reader, "Difficulty", difficulty);
		instrumentDamage = DefaultDataReadUtility.Read(reader, "InstrumentDamage", instrumentDamage);
		position = EngineDataReadUtility.Read(reader, "Position", position);
		anchor = EngineDataReadUtility.Read(reader, "Anchor", anchor);
		pivot = EngineDataReadUtility.Read(reader, "Pivot", pivot);
		imageBackground = UnityDataReadUtility.Read(reader, "ImageBackground", imageBackground);
		imageInstrument = UnityDataReadUtility.Read(reader, "ImageInstrument", imageInstrument);
		imageLock = UnityDataReadUtility.Read(reader, "ImageLock", imageLock);
		imageNotAvailable = UnityDataReadUtility.Read(reader, "ImageNotAvailable", imageNotAvailable);
		limitations = DefaultDataReadUtility.ReadListEnum(reader, "Limitations", limitations);
		except = DefaultDataReadUtility.ReadListEnum(reader, "Except", except);
		openTime = DefaultDataReadUtility.Read(reader, "OpenTime", openTime);
		imageForeground = UnityDataReadUtility.Read(reader, "ImageForeground", imageForeground);
		openStartAudio = UnityDataReadUtility.Read(reader, "OpenStartAudio", openStartAudio);
		openProgressAudio = UnityDataReadUtility.Read(reader, "OpenProgressAudio", openProgressAudio);
		openCompleteAudio = UnityDataReadUtility.Read(reader, "OpenCompleteAudio", openCompleteAudio);
		openCancelAudio = UnityDataReadUtility.Read(reader, "OpenCancelAudio", openCancelAudio);
	}
}