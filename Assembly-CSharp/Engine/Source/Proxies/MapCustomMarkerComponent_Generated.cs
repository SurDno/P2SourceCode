using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Maps;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MapCustomMarkerComponent))]
public class MapCustomMarkerComponent_Generated :
	MapCustomMarkerComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<MapCustomMarkerComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((MapCustomMarkerComponent_Generated)target2).isEnabled = isEnabled;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		UnityDataWriteUtility.Write(writer, "Position", position);
	}

	public void StateLoad(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		position = UnityDataReadUtility.Read(reader, "Position", position);
	}
}