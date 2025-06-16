using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(PositionComponent))]
public class PositionComponent_Generated :
	PositionComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<PositionComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (PositionComponent_Generated)target2;
		componentGenerated.position = position;
		componentGenerated.rotation = rotation;
	}

	public void DataWrite(IDataWriter writer) {
		UnityDataWriteUtility.Write(writer, "Position", position);
		UnityDataWriteUtility.Write(writer, "Rotation", rotation);
	}

	public void DataRead(IDataReader reader, Type type) {
		position = UnityDataReadUtility.Read(reader, "Position", position);
		rotation = UnityDataReadUtility.Read(reader, "Rotation", rotation);
	}
}