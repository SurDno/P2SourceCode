using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(BlueprintEffect))]
public class BlueprintEffect_Generated :
	BlueprintEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<BlueprintEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var blueprintEffectGenerated = (BlueprintEffect_Generated)target2;
		blueprintEffectGenerated.name = name;
		blueprintEffectGenerated.queue = queue;
		blueprintEffectGenerated.blueprint = blueprint;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", name);
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		UnityDataWriteUtility.Write(writer, "Blueprint", blueprint);
	}

	public void DataRead(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.Read(reader, "Name", name);
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		blueprint = UnityDataReadUtility.Read(reader, "Blueprint", blueprint);
	}
}