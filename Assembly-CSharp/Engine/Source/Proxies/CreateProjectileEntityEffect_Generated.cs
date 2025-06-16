using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(CreateProjectileEntityEffect))]
public class CreateProjectileEntityEffect_Generated :
	CreateProjectileEntityEffect,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<CreateProjectileEntityEffect_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var entityEffectGenerated = (CreateProjectileEntityEffect_Generated)target2;
		entityEffectGenerated.queue = queue;
		entityEffectGenerated.template = template;
		entityEffectGenerated.spawnPlace = spawnPlace;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
		UnityDataWriteUtility.Write(writer, "Template", template);
		DefaultDataWriteUtility.WriteEnum(writer, "SpawnPlace", spawnPlace);
	}

	public void DataRead(IDataReader reader, Type type) {
		queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
		template = UnityDataReadUtility.Read(reader, "Template", template);
		spawnPlace = DefaultDataReadUtility.ReadEnum<ProjectileSpawnPlaceEnum>(reader, "SpawnPlace");
	}
}