using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      FlameAbilityProjectile_Generated projectileGenerated = (FlameAbilityProjectile_Generated) target2;
      projectileGenerated.radius = this.radius;
      projectileGenerated.hitAngle = this.hitAngle;
      projectileGenerated.flameEffectiveTime = this.flameEffectiveTime;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "EnemyHitRadius", this.radius);
      DefaultDataWriteUtility.Write(writer, "EnemyHitAngle", this.hitAngle);
      DefaultDataWriteUtility.Write(writer, "FlameEffectiveTime", this.flameEffectiveTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.radius = DefaultDataReadUtility.Read(reader, "EnemyHitRadius", this.radius);
      this.hitAngle = DefaultDataReadUtility.Read(reader, "EnemyHitAngle", this.hitAngle);
      this.flameEffectiveTime = DefaultDataReadUtility.Read(reader, "FlameEffectiveTime", this.flameEffectiveTime);
    }
  }
}
