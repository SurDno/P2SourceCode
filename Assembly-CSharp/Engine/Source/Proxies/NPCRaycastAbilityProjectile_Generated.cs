// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.NPCRaycastAbilityProjectile_Generated
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NPCRaycastAbilityProjectile_Generated projectileGenerated = (NPCRaycastAbilityProjectile_Generated) target2;
      projectileGenerated.hitDistance = this.hitDistance;
      projectileGenerated.bulletsCount = this.bulletsCount;
      projectileGenerated.minimumDirectionScatter = this.minimumDirectionScatter;
      projectileGenerated.maximumDirectionScatter = this.maximumDirectionScatter;
      projectileGenerated.maximumAimingDeltaAngle = this.maximumAimingDeltaAngle;
      projectileGenerated.bulletScatter = this.bulletScatter;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "HitDistance", this.hitDistance);
      DefaultDataWriteUtility.Write(writer, "BulletsCount", this.bulletsCount);
      DefaultDataWriteUtility.Write(writer, "MinimumDirectionScatter", this.minimumDirectionScatter);
      DefaultDataWriteUtility.Write(writer, "MaximumDirectionScatter", this.maximumDirectionScatter);
      DefaultDataWriteUtility.Write(writer, "MaximumAimingDeltaAngle", this.maximumAimingDeltaAngle);
      DefaultDataWriteUtility.Write(writer, "BulletScatter", this.bulletScatter);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.hitDistance = DefaultDataReadUtility.Read(reader, "HitDistance", this.hitDistance);
      this.bulletsCount = DefaultDataReadUtility.Read(reader, "BulletsCount", this.bulletsCount);
      this.minimumDirectionScatter = DefaultDataReadUtility.Read(reader, "MinimumDirectionScatter", this.minimumDirectionScatter);
      this.maximumDirectionScatter = DefaultDataReadUtility.Read(reader, "MaximumDirectionScatter", this.maximumDirectionScatter);
      this.maximumAimingDeltaAngle = DefaultDataReadUtility.Read(reader, "MaximumAimingDeltaAngle", this.maximumAimingDeltaAngle);
      this.bulletScatter = DefaultDataReadUtility.Read(reader, "BulletScatter", this.bulletScatter);
    }
  }
}
