using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(BlueprintComponent))]
public class BlueprintComponent_Generated :
	BlueprintComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<BlueprintComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (BlueprintComponent_Generated)target2;
		componentGenerated.isEnabled = isEnabled;
		componentGenerated.blueprintResource = blueprintResource;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		UnityDataWriteUtility.Write(writer, "Blueprint", blueprintResource);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		blueprintResource = UnityDataReadUtility.Read(reader, "Blueprint", blueprintResource);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		UnityDataWriteUtility.Write(writer, "BlueprintResource", blueprintResource);
	}

	public void StateLoad(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		blueprintResource = UnityDataReadUtility.Read(reader, "BlueprintResource", blueprintResource);
	}
}