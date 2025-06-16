using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Model))]
public class Model_Generated :
	Model,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var modelGenerated = (Model_Generated)target2;
		modelGenerated.name = name;
		modelGenerated.connection = connection;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		UnityDataWriteUtility.Write(writer, "Connection", connection);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		connection = UnityDataReadUtility.Read(reader, "Connection", connection);
	}
}