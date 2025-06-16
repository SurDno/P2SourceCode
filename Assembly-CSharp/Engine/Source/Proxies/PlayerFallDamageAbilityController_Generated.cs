using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(PlayerFallDamageAbilityController))]
public class PlayerFallDamageAbilityController_Generated :
	PlayerFallDamageAbilityController,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<PlayerFallDamageAbilityController_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var controllerGenerated = (PlayerFallDamageAbilityController_Generated)target2;
		controllerGenerated.minFall = minFall;
		controllerGenerated.maxFall = maxFall;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "minFall", minFall);
		DefaultDataWriteUtility.Write(writer, "maxFall", maxFall);
	}

	public void DataRead(IDataReader reader, Type type) {
		minFall = DefaultDataReadUtility.Read(reader, "minFall", minFall);
		maxFall = DefaultDataReadUtility.Read(reader, "maxFall", maxFall);
	}
}