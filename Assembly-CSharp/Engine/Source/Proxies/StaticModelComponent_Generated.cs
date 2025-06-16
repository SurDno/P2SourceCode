using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(StaticModelComponent))]
public class StaticModelComponent_Generated :
	StaticModelComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<StaticModelComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (StaticModelComponent_Generated)target2;
		componentGenerated.relativePosition = relativePosition;
		componentGenerated.connection = connection;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "RelativePosition", relativePosition);
		UnityDataWriteUtility.Write(writer, "Connection", connection);
	}

	public void DataRead(IDataReader reader, Type type) {
		relativePosition = DefaultDataReadUtility.Read(reader, "RelativePosition", relativePosition);
		connection = UnityDataReadUtility.Read(reader, "Connection", connection);
	}
}