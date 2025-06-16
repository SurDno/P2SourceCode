using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Saves;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(StorableComponent))]
public class StorableComponent_Generated :
	StorableComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<StorableComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (StorableComponent_Generated)target2;
		componentGenerated.isEnabled = isEnabled;
		componentGenerated.placeholder = placeholder;
		CloneableObjectUtility.FillListTo(componentGenerated.groups, groups);
		CloneableObjectUtility.CopyListTo(componentGenerated.tooltips, tooltips);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		UnityDataWriteUtility.Write(writer, "Placeholder", placeholder);
		DefaultDataWriteUtility.WriteListEnum(writer, "Groups", groups);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Tooltips", tooltips);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		placeholder = UnityDataReadUtility.Read(reader, "Placeholder", placeholder);
		groups = DefaultDataReadUtility.ReadListEnum(reader, "Groups", groups);
		tooltips = DefaultDataReadUtility.ReadListSerialize(reader, "Tooltips", tooltips);
	}

	public void StateSave(IDataWriter writer) {
		DefaultStateSaveUtility.SaveSerialize(writer, "Cell", Cell);
		EngineDataWriteUtility.Write(writer, "Title", Title);
		EngineDataWriteUtility.Write(writer, "Tooltip", Tooltip);
		EngineDataWriteUtility.Write(writer, "Description", Description);
		EngineDataWriteUtility.Write(writer, "SpecialDescription", SpecialDescription);
		DefaultStateSaveUtility.SaveSerialize(writer, "StorableData", StorableData);
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		DefaultDataWriteUtility.Write(writer, "Count", count);
		DefaultDataWriteUtility.Write(writer, "Max", max);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Cell = DefaultStateLoadUtility.ReadSerialize<Cell>(reader, "Cell");
		Title = EngineDataReadUtility.Read(reader, "Title", Title);
		Tooltip = EngineDataReadUtility.Read(reader, "Tooltip", Tooltip);
		Description = EngineDataReadUtility.Read(reader, "Description", Description);
		SpecialDescription = EngineDataReadUtility.Read(reader, "SpecialDescription", SpecialDescription);
		StorableData = DefaultStateLoadUtility.ReadSerialize<StorableData>(reader, "StorableData");
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		count = DefaultDataReadUtility.Read(reader, "Count", count);
		max = DefaultDataReadUtility.Read(reader, "Max", max);
	}
}