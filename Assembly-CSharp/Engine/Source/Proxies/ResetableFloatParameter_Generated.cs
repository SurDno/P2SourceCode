using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ResetableFloatParameter))]
public class ResetableFloatParameter_Generated :
	ResetableFloatParameter,
	IComputeNeedSave,
	INeedSave,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public bool NeedSave { get; private set; } = true;

	public void ComputeNeedSave(object target2) {
		NeedSave = true;
		var parameterGenerated = (ResetableFloatParameter_Generated)target2;
		if (parameterGenerated.name != name || parameterGenerated.value != (double)value ||
		    parameterGenerated.baseValue != (double)baseValue || parameterGenerated.minValue != (double)minValue ||
		    parameterGenerated.maxValue != (double)maxValue)
			return;
		NeedSave = false;
	}

	public object Clone() {
		var instance = Activator.CreateInstance<ResetableFloatParameter_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var parameterGenerated = (ResetableFloatParameter_Generated)target2;
		parameterGenerated.name = name;
		parameterGenerated.value = value;
		parameterGenerated.baseValue = baseValue;
		parameterGenerated.minValue = minValue;
		parameterGenerated.maxValue = maxValue;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
		DefaultDataWriteUtility.Write(writer, "Value", value);
		DefaultDataWriteUtility.Write(writer, "BaseValue", baseValue);
		DefaultDataWriteUtility.Write(writer, "MinValue", minValue);
		DefaultDataWriteUtility.Write(writer, "MaxValue", maxValue);
	}

	public void DataRead(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Name");
		value = DefaultDataReadUtility.Read(reader, "Value", value);
		baseValue = DefaultDataReadUtility.Read(reader, "BaseValue", baseValue);
		minValue = DefaultDataReadUtility.Read(reader, "MinValue", minValue);
		maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", maxValue);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
		DefaultDataWriteUtility.Write(writer, "Value", value);
		DefaultDataWriteUtility.Write(writer, "BaseValue", baseValue);
		DefaultDataWriteUtility.Write(writer, "MinValue", minValue);
		DefaultDataWriteUtility.Write(writer, "MaxValue", maxValue);
	}

	public void StateLoad(IDataReader reader, Type type) {
		value = DefaultDataReadUtility.Read(reader, "Value", value);
		baseValue = DefaultDataReadUtility.Read(reader, "BaseValue", baseValue);
		minValue = DefaultDataReadUtility.Read(reader, "MinValue", minValue);
		maxValue = DefaultDataReadUtility.Read(reader, "MaxValue", maxValue);
	}
}