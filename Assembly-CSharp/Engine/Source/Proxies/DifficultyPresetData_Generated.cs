using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(DifficultyPresetData))]
public class DifficultyPresetData_Generated :
	DifficultyPresetData,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<DifficultyPresetData_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var presetDataGenerated = (DifficultyPresetData_Generated)target2;
		presetDataGenerated.Name = Name;
		CloneableObjectUtility.CopyListTo(presetDataGenerated.Items, Items);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Items", Items);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
		Items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", Items);
	}
}