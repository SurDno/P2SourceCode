using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ConditionIntOperation))]
public class ConditionIntOperation_Generated :
	ConditionIntOperation,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ConditionIntOperation_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var operationGenerated = (ConditionIntOperation_Generated)target2;
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
		trueResult = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "True");
		falseResult = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "False");
	}
}