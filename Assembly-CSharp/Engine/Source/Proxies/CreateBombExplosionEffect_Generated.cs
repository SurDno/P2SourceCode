using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(CreateBombExplosionEffect))]
public class CreateBombExplosionEffect_Generated :
	CreateBombExplosionEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<CreateBombExplosionEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var explosionEffectGenerated = (CreateBombExplosionEffect_Generated)target2;
		explosionEffectGenerated.queue = queue;
		explosionEffectGenerated.template = template;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		UnityDataWriteUtility.Write(writer, "Template", template);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		template = UnityDataReadUtility.Read(reader, "Template", template);
	}
}