using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(StorableTooltipItemDurability))]
public class StorableTooltipItemDurability_Generated :
	StorableTooltipItemDurability,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<StorableTooltipItemDurability_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var durabilityGenerated = (StorableTooltipItemDurability_Generated)target2;
		durabilityGenerated.isEnabled = isEnabled;
		durabilityGenerated.name = name;
		durabilityGenerated.parameter = parameter;
		durabilityGenerated.isFood = isFood;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		DefaultDataWriteUtility.WriteEnum(writer, "Name", name);
		DefaultDataWriteUtility.WriteEnum(writer, "Parameter", parameter);
		DefaultDataWriteUtility.Write(writer, "IsFood", isFood);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		name = DefaultDataReadUtility.ReadEnum<StorableTooltipNameEnum>(reader, "Name");
		parameter = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Parameter");
		isFood = DefaultDataReadUtility.Read(reader, "IsFood", isFood);
	}
}