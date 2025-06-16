// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RaycastAbilityProjectile_Generated
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
  [FactoryProxy(typeof (RaycastAbilityProjectile))]
  public class RaycastAbilityProjectile_Generated : 
    RaycastAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RaycastAbilityProjectile_Generated instance = Activator.CreateInstance<RaycastAbilityProjectile_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RaycastAbilityProjectile_Generated projectileGenerated = (RaycastAbilityProjectile_Generated) target2;
      projectileGenerated.hitDistance = this.hitDistance;
      projectileGenerated.bulletsCount = this.bulletsCount;
      projectileGenerated.minimumAimingDeltaAngle = this.minimumAimingDeltaAngle;
      projectileGenerated.maximumAimingDeltaAngle = this.maximumAimingDeltaAngle;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "HitDistance", this.hitDistance);
      DefaultDataWriteUtility.Write(writer, "BulletsCount", this.bulletsCount);
      DefaultDataWriteUtility.Write(writer, "MinimumAimingDeltaAngle", this.minimumAimingDeltaAngle);
      DefaultDataWriteUtility.Write(writer, "MaximumAimingDeltaAngle", this.maximumAimingDeltaAngle);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.hitDistance = DefaultDataReadUtility.Read(reader, "HitDistance", this.hitDistance);
      this.bulletsCount = DefaultDataReadUtility.Read(reader, "BulletsCount", this.bulletsCount);
      this.minimumAimingDeltaAngle = DefaultDataReadUtility.Read(reader, "MinimumAimingDeltaAngle", this.minimumAimingDeltaAngle);
      this.maximumAimingDeltaAngle = DefaultDataReadUtility.Read(reader, "MaximumAimingDeltaAngle", this.maximumAimingDeltaAngle);
    }
  }
}
