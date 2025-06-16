using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(HolsterAbilityController))]
public class HolsterAbilityController_Generated :
	HolsterAbilityController,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<HolsterAbilityController_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((HolsterAbilityController_Generated)target2).weaponKind = weaponKind;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Weapon", weaponKind);
	}

	public void DataRead(IDataReader reader, Type type) {
		weaponKind = DefaultDataReadUtility.ReadEnum<WeaponKind>(reader, "Weapon");
	}
}