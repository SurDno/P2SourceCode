using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(NpcPunchLowStamina))]
public class NpcPunchLowStamina_Generated :
	NpcPunchLowStamina,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<NpcPunchLowStamina_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var staminaGenerated = (NpcPunchLowStamina_Generated)target2;
		staminaGenerated.name = name;
		staminaGenerated.queue = queue;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", name);
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
	}

	public void DataRead(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.Read(reader, "Name", name);
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
	}
}