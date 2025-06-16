using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ConditionFloatOperation))]
public class ConditionFloatOperation_Generated :
	ConditionFloatOperation,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ConditionFloatOperation_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var operationGenerated = (ConditionFloatOperation_Generated)target2;
		operationGenerated.condition = CloneableObjectUtility.Clone(condition);
		operationGenerated.trueResult = CloneableObjectUtility.Clone(trueResult);
		operationGenerated.falseResult = CloneableObjectUtility.Clone(falseResult);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "Condition", condition);
		DefaultDataWriteUtility.WriteSerialize(writer, "True", trueResult);
		DefaultDataWriteUtility.WriteSerialize(writer, "False", falseResult);
	}

	public void DataRead(IDataReader reader, Type type) {
		condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
		trueResult = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "True");
		falseResult = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "False");
	}
}