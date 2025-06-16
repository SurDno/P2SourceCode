using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(CloseCombatAbilityProjectile))]
public class CloseCombatAbilityProjectile_Generated :
	CloseCombatAbilityProjectile,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<CloseCombatAbilityProjectile_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var projectileGenerated = (CloseCombatAbilityProjectile_Generated)target2;
		projectileGenerated.blocked = blocked;
		projectileGenerated.orientation = orientation;
		projectileGenerated.radius = radius;
		projectileGenerated.angle = angle;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Blocked", blocked);
		DefaultDataWriteUtility.WriteEnum(writer, "Orientation", orientation);
		DefaultDataWriteUtility.Write(writer, "Radius", radius);
		DefaultDataWriteUtility.Write(writer, "Angle", angle);
	}

	public void DataRead(IDataReader reader, Type type) {
		blocked = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Blocked");
		orientation = DefaultDataReadUtility.ReadEnum<HitOrientationTypeEnum>(reader, "Orientation");
		radius = DefaultDataReadUtility.Read(reader, "Radius", radius);
		angle = DefaultDataReadUtility.Read(reader, "Angle", angle);
	}
}