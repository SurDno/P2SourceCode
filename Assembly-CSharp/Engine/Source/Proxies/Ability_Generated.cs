using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons.Abilities;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Ability))]
public class Ability_Generated :
	Ability,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var abilityGenerated = (Ability_Generated)target2;
		abilityGenerated.name = name;
		CloneableObjectUtility.CopyListTo(abilityGenerated.items, items);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteListSerialize(writer, "AbilityItems", items);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		items = DefaultDataReadUtility.ReadListSerialize(reader, "AbilityItems", items);
	}
}