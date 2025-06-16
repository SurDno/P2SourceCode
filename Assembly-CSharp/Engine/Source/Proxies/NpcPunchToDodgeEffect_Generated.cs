using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(NpcPunchToDodgeEffect))]
public class NpcPunchToDodgeEffect_Generated :
	NpcPunchToDodgeEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<NpcPunchToDodgeEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var dodgeEffectGenerated = (NpcPunchToDodgeEffect_Generated)target2;
		dodgeEffectGenerated.name = name;
		dodgeEffectGenerated.punchEnum = punchEnum;
		dodgeEffectGenerated.queue = queue;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", name);
		DefaultDataWriteUtility.WriteEnum(writer, "punchType", punchEnum);
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
	}

	public void DataRead(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.Read(reader, "Name", name);
		punchEnum = DefaultDataReadUtility.ReadEnum<PunchTypeEnum>(reader, "punchType");
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
	}
}