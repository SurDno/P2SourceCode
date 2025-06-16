using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Effects.Values;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(AbilityValueInfo))]
public class AbilityValueInfo_Generated :
	AbilityValueInfo,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<AbilityValueInfo_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var valueInfoGenerated = (AbilityValueInfo_Generated)target2;
		valueInfoGenerated.Name = Name;
		valueInfoGenerated.Value = CloneableObjectUtility.Clone(Value);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Name", Name);
		DefaultDataWriteUtility.WriteSerialize(writer, "Value", Value);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.ReadEnum<AbilityValueNameEnum>(reader, "Name");
		Value = DefaultDataReadUtility.ReadSerialize<IAbilityValue>(reader, "Value");
	}
}