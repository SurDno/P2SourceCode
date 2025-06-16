// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.AbilityItem_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AbilityItem))]
  public class AbilityItem_Generated : 
    AbilityItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AbilityItem_Generated instance = Activator.CreateInstance<AbilityItem_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AbilityItem_Generated abilityItemGenerated = (AbilityItem_Generated) target2;
      abilityItemGenerated.name = this.name;
      abilityItemGenerated.controller = CloneableObjectUtility.Clone<IAbilityController>(this.controller);
      abilityItemGenerated.projectile = CloneableObjectUtility.Clone<IAbilityProjectile>(this.projectile);
      abilityItemGenerated.targets = this.targets;
      CloneableObjectUtility.CopyListTo<IEffect>(abilityItemGenerated.effects, this.effects);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteSerialize<IAbilityController>(writer, "AbilityController", this.controller);
      DefaultDataWriteUtility.WriteSerialize<IAbilityProjectile>(writer, "AbilityProjectile", this.projectile);
      DefaultDataWriteUtility.WriteEnum<AbilityTargetEnum>(writer, "Targets", this.targets);
      DefaultDataWriteUtility.WriteListSerialize<IEffect>(writer, "Effects", this.effects);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.controller = DefaultDataReadUtility.ReadSerialize<IAbilityController>(reader, "AbilityController");
      this.projectile = DefaultDataReadUtility.ReadSerialize<IAbilityProjectile>(reader, "AbilityProjectile");
      this.targets = DefaultDataReadUtility.ReadEnum<AbilityTargetEnum>(reader, "Targets");
      this.effects = DefaultDataReadUtility.ReadListSerialize<IEffect>(reader, "Effects", this.effects);
    }
  }
}
