// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.NPCShotAbilityProjectile_Generated
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
  [FactoryProxy(typeof (NPCShotAbilityProjectile))]
  public class NPCShotAbilityProjectile_Generated : 
    NPCShotAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NPCShotAbilityProjectile_Generated instance = Activator.CreateInstance<NPCShotAbilityProjectile_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((NPCShotAbilityProjectile) target2).blocked = this.blocked;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<BlockTypeEnum>(writer, "Blocked", this.blocked);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.blocked = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Blocked");
    }
  }
}
