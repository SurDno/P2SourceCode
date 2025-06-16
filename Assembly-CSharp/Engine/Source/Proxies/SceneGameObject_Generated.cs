using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Connections;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(SceneGameObject))]
public class SceneGameObject_Generated :
	SceneGameObject,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<SceneGameObject_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((SceneGameObject_Generated)target2).id = id;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
	}
}