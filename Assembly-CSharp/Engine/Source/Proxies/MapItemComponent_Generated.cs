using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Maps;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MapItemComponent))]
public class MapItemComponent_Generated :
	MapItemComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<MapItemComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (MapItemComponent_Generated)target2;
		componentGenerated.isEnabled = isEnabled;
		componentGenerated.placeholder = placeholder;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		UnityDataWriteUtility.Write(writer, "Placeholder", placeholder);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		placeholder = UnityDataReadUtility.Read(reader, "Placeholder", placeholder);
	}

	public void StateSave(IDataWriter writer) {
		CustomStateSaveUtility.SaveReference(writer, "BoundCharacter", BoundCharacter);
		EngineDataWriteUtility.Write(writer, "Text", Text);
		EngineDataWriteUtility.Write(writer, "TooltipText", TooltipText);
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		CustomStateSaveUtility.SaveListReferences(writer, "Nodes", nodes);
		CustomStateSaveUtility.SaveReference(writer, "TooltipResource", tooltipResource);
		DefaultDataWriteUtility.Write(writer, "Discovered", discovered);
		EngineDataWriteUtility.Write(writer, "Title", title);
	}

	public void StateLoad(IDataReader reader, Type type) {
		BoundCharacter = CustomStateLoadUtility.LoadReference(reader, "BoundCharacter", BoundCharacter);
		Text = EngineDataReadUtility.Read(reader, "Text", Text);
		TooltipText = EngineDataReadUtility.Read(reader, "TooltipText", TooltipText);
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		nodes = CustomStateLoadUtility.LoadListReferences(reader, "Nodes", nodes);
		tooltipResource = CustomStateLoadUtility.LoadReference(reader, "TooltipResource", tooltipResource);
		discovered = DefaultDataReadUtility.Read(reader, "Discovered", discovered);
		title = EngineDataReadUtility.Read(reader, "Title", title);
	}
}