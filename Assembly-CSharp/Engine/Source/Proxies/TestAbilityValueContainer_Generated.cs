using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons.Abilities;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(TestAbilityValueContainer))]
public class TestAbilityValueContainer_Generated :
	TestAbilityValueContainer,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var containerGenerated = (TestAbilityValueContainer_Generated)target2;
		containerGenerated.name = name;
		CloneableObjectUtility.CopyListTo(containerGenerated.values, values);
		containerGenerated.blueprint = blueprint;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Values", values);
		UnityDataWriteUtility.Write(writer, "Blueprint", blueprint);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		values = DefaultDataReadUtility.ReadListSerialize(reader, "Values", values);
		blueprint = UnityDataReadUtility.Read(reader, "Blueprint", blueprint);
	}
}