// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.FlameAbilityProjectile_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;
using System;

#nullable disable
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
