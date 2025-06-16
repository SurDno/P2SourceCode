using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(DifficultyGroupItemData))]
public class DifficultyGroupItemData_Generated :
	DifficultyGroupItemData,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<DifficultyGroupItemData_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((DifficultyGroupItemData)target2).Name = Name;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
	}
}