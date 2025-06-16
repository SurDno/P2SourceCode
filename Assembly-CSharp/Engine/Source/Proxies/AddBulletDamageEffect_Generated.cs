// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.AddBulletDamageEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AddBulletDamageEffect))]
  public class AddBulletDamageEffect_Generated : 
    AddBulletDamageEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AddBulletDamageEffect_Generated instance = Activator.CreateInstance<AddBulletDamageEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AddBulletDamageEffect_Generated damageEffectGenerated = (AddBulletDamageEffect_Generated) target2;
      damageEffectGenerated.queue = this.queue;
      damageEffectGenerated.enable = this.enable;
      damageEffectGenerated.damageParameterName = this.damageParameterName;
      damageEffectGenerated.difficultyMultiplierParameterName = this.difficultyMultiplierParameterName;
      damageEffectGenerated.bodyDamage = this.bodyDamage;
      damageEffectGenerated.armDamage = this.armDamage;
      damageEffectGenerated.legDamage = this.legDamage;
      damageEffectGenerated.headDamage = this.headDamage;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "DamageParameterName", this.damageParameterName);
      DefaultDataWriteUtility.Write(writer, "DifficultyMultiplierParameterName", this.difficultyMultiplierParameterName);
      DefaultDataWriteUtility.Write(writer, "BodyDamage", this.bodyDamage);
      DefaultDataWriteUtility.Write(writer, "ArmDamage", this.armDamage);
      DefaultDataWriteUtility.Write(writer, "LegDamage", this.legDamage);
      DefaultDataWriteUtility.Write(writer, "HeadDamage", this.headDamage);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.damageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "DamageParameterName");
      this.difficultyMultiplierParameterName = DefaultDataReadUtility.Read(reader, "DifficultyMultiplierParameterName", this.difficultyMultiplierParameterName);
      this.bodyDamage = DefaultDataReadUtility.Read(reader, "BodyDamage", this.bodyDamage);
      this.armDamage = DefaultDataReadUtility.Read(reader, "ArmDamage", this.armDamage);
      this.legDamage = DefaultDataReadUtility.Read(reader, "LegDamage", this.legDamage);
      this.headDamage = DefaultDataReadUtility.Read(reader, "HeadDamage", this.headDamage);
    }
  }
}
