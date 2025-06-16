using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.AssetDatabaseService;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(AssetDatabaseMapItemData))]
public class AssetDatabaseMapItemData_Generated :
	AssetDatabaseMapItemData,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<AssetDatabaseMapItemData_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var itemDataGenerated = (AssetDatabaseMapItemData_Generated)target2;
		itemDataGenerated.Id = Id;
		itemDataGenerated.Name = Name;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", Id);
		DefaultDataWriteUtility.Write(writer, "Name", Name);
	}

	public void DataRead(IDataReader reader, Type type) {
		Id = DefaultDataReadUtility.Read(reader, "Id", Id);
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
	}
}