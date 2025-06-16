using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NPCRaycastAbilityProjectile))]
  public class NPCRaycastAbilityProjectile_Generated : 
    NPCRaycastAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NPCRaycastAbilityProjectile_Generated instance = Activator.CreateInstance<NPCRaycastAbilityProjectile_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      NPCRaycastAbilityProjectile_Generated projectileGenerated = (NPCRaycastAbilityProjectile_Generated) target2;
      projectileGenerated.hitDistance = hitDistance;
      projectileGenerated.bulletsCount = bulletsCount;
      projectileGenerated.minimumDirectionScatter = minimumDirectionScatter;
      projectileGenerated.maximumDirectionScatter = maximumDirectionScatter;
      projectileGenerated.maximumAimingDeltaAngle = maximumAimingDeltaAngle;
      projectileGenerated.bulletScatter = bulletScatter;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "HitDistance", hitDistance);
      DefaultDataWriteUtility.Write(writer, "BulletsCount", bulletsCount);
      DefaultDataWriteUtility.Write(writer, "MinimumDirectionScatter", minimumDirectionScatter);
      DefaultDataWriteUtility.Write(writer, "MaximumDirectionScatter", maximumDirectionScatter);
      DefaultDataWriteUtility.Write(writer, "MaximumAimingDeltaAngle", maximumAimingDeltaAngle);
      DefaultDataWriteUtility.Write(writer, "BulletScatter", bulletScatter);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      hitDistance = DefaultDataReadUtility.Read(reader, "HitDistance", hitDistance);
      bulletsCount = DefaultDataReadUtility.Read(reader, "BulletsCount", bulletsCount);
      minimumDirectionScatter = DefaultDataReadUtility.Read(reader, "MinimumDirectionScatter", minimumDirectionScatter);
      maximumDirectionScatter = DefaultDataReadUtility.Read(reader, "MaximumDirectionScatter", maximumDirectionScatter);
      maximumAimingDeltaAngle = DefaultDataReadUtility.Read(reader, "MaximumAimingDeltaAngle", maximumAimingDeltaAngle);
      bulletScatter = DefaultDataReadUtility.Read(reader, "BulletScatter", bulletScatter);
    }
  }
}
