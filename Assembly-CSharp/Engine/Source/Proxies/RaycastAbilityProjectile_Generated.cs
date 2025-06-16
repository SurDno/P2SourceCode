using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(RaycastAbilityProjectile))]
public class RaycastAbilityProjectile_Generated :
	RaycastAbilityProjectile,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<RaycastAbilityProjectile_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var projectileGenerated = (RaycastAbilityProjectile_Generated)target2;
		projectileGenerated.hitDistance = hitDistance;
		projectileGenerated.bulletsCount = bulletsCount;
		projectileGenerated.minimumAimingDeltaAngle = minimumAimingDeltaAngle;
		projectileGenerated.maximumAimingDeltaAngle = maximumAimingDeltaAngle;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "HitDistance", hitDistance);
		DefaultDataWriteUtility.Write(writer, "BulletsCount", bulletsCount);
		DefaultDataWriteUtility.Write(writer, "MinimumAimingDeltaAngle", minimumAimingDeltaAngle);
		DefaultDataWriteUtility.Write(writer, "MaximumAimingDeltaAngle", maximumAimingDeltaAngle);
	}

	public void DataRead(IDataReader reader, Type type) {
		hitDistance = DefaultDataReadUtility.Read(reader, "HitDistance", hitDistance);
		bulletsCount = DefaultDataReadUtility.Read(reader, "BulletsCount", bulletsCount);
		minimumAimingDeltaAngle =
			DefaultDataReadUtility.Read(reader, "MinimumAimingDeltaAngle", minimumAimingDeltaAngle);
		maximumAimingDeltaAngle =
			DefaultDataReadUtility.Read(reader, "MaximumAimingDeltaAngle", maximumAimingDeltaAngle);
	}
}