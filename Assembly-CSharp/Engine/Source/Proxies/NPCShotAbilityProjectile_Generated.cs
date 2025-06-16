using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(NPCShotAbilityProjectile))]
public class NPCShotAbilityProjectile_Generated :
	NPCShotAbilityProjectile,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<NPCShotAbilityProjectile_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((NPCShotAbilityProjectile_Generated)target2).blocked = blocked;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Blocked", blocked);
	}

	public void DataRead(IDataReader reader, Type type) {
		blocked = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Blocked");
	}
}