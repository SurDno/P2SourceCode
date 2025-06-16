using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Connections;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(IndoorCrowdComponent))]
public class IndoorCrowdComponent_Generated :
	IndoorCrowdComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<IndoorCrowdComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (IndoorCrowdComponent_Generated)target2;
		componentGenerated.region = CloneableObjectUtility.Clone(region);
		CloneableObjectUtility.CopyListTo(componentGenerated.areas, areas);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "Region", region);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Areas", areas);
	}

	public void DataRead(IDataReader reader, Type type) {
		region = DefaultDataReadUtility.ReadSerialize<SceneGameObject>(reader, "Region");
		areas = DefaultDataReadUtility.ReadListSerialize(reader, "Areas", areas);
	}
}