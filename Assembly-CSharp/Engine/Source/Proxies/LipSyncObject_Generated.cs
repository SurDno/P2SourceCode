using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(LipSyncObject))]
public class LipSyncObject_Generated :
	LipSyncObject,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var syncObjectGenerated = (LipSyncObject_Generated)target2;
		syncObjectGenerated.name = name;
		CloneableObjectUtility.CopyListTo(syncObjectGenerated.languages, languages);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Languages", languages);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		languages = DefaultDataReadUtility.ReadListSerialize(reader, "Languages", languages);
	}
}