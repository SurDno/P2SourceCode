// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RadiusAbilityProjectile_Generated
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
  [FactoryProxy(typeof (RadiusAbilityProjectile))]
  public class RadiusAbilityProjectile_Generated : 
    RadiusAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RadiusAbilityProjectile_Generated instance = Activator.CreateInstance<RadiusAbilityProjectile_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RadiusAbilityProjectile_Generated projectileGenerated = (RadiusAbilityProjectile_Generated) target2;
      projectileGenerated.radius = this.radius;
      projectileGenerated.ignoreSelf = this.ignoreSelf;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Radius", this.radius);
      DefaultDataWriteUtility.Write(writer, "IgnoreSelf", this.ignoreSelf);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.radius = DefaultDataReadUtility.Read(reader, "Radius", this.radius);
      this.ignoreSelf = DefaultDataReadUtility.Read(reader, "IgnoreSelf", this.ignoreSelf);
    }
  }
}
