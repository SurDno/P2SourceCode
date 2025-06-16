using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Values;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(FloatAbilityValue))]
public class FloatAbilityValue_Generated :
	FloatAbilityValue,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<FloatAbilityValue_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((FloatAbilityValue_Generated)target2).value = value;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Value", value);
	}

	public void DataRead(IDataReader reader, Type type) {
		value = DefaultDataReadUtility.Read(reader, "Value", value);
	}
}