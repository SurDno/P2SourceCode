using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FlameAbilityProjectile))]
  public class FlameAbilityProjectile_Generated : 
    FlameAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FlameAbilityProjectile_Generated instance = Activator.CreateInstance<FlameAbilityProjectile_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      FlameAbilityProjectile_Generated projectileGenerated = (FlameAbilityProjectile_Generated) target2;
      projectileGenerated.radius = radius;
      projectileGenerated.hitAngle = hitAngle;
      projectileGenerated.flameEffectiveTime = flameEffectiveTime;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "EnemyHitRadius", radius);
      DefaultDataWriteUtility.Write(writer, "EnemyHitAngle", hitAngle);
      DefaultDataWriteUtility.Write(writer, "FlameEffectiveTime", flameEffectiveTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      radius = DefaultDataReadUtility.Read(reader, "EnemyHitRadius", radius);
      hitAngle = DefaultDataReadUtility.Read(reader, "EnemyHitAngle", hitAngle);
      flameEffectiveTime = DefaultDataReadUtility.Read(reader, "FlameEffectiveTime", flameEffectiveTime);
    }
  }
}
