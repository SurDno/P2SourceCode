// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.NpcSetWeaponEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcSetWeaponEffect))]
  public class NpcSetWeaponEffect_Generated : 
    NpcSetWeaponEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcSetWeaponEffect_Generated instance = Activator.CreateInstance<NpcSetWeaponEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NpcSetWeaponEffect_Generated weaponEffectGenerated = (NpcSetWeaponEffect_Generated) target2;
      weaponEffectGenerated.queue = this.queue;
      weaponEffectGenerated.weapon = this.weapon;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.WriteEnum<WeaponEnum>(writer, "Weapon", this.weapon);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.weapon = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "Weapon");
    }
  }
}
