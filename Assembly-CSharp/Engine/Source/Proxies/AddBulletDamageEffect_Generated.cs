using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      AddBulletDamageEffect_Generated damageEffectGenerated = (AddBulletDamageEffect_Generated) target2;
      damageEffectGenerated.queue = queue;
      damageEffectGenerated.enable = enable;
      damageEffectGenerated.damageParameterName = damageParameterName;
      damageEffectGenerated.difficultyMultiplierParameterName = difficultyMultiplierParameterName;
      damageEffectGenerated.bodyDamage = bodyDamage;
      damageEffectGenerated.armDamage = armDamage;
      damageEffectGenerated.legDamage = legDamage;
      damageEffectGenerated.headDamage = headDamage;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.WriteEnum(writer, "DamageParameterName", damageParameterName);
      DefaultDataWriteUtility.Write(writer, "DifficultyMultiplierParameterName", difficultyMultiplierParameterName);
      DefaultDataWriteUtility.Write(writer, "BodyDamage", bodyDamage);
      DefaultDataWriteUtility.Write(writer, "ArmDamage", armDamage);
      DefaultDataWriteUtility.Write(writer, "LegDamage", legDamage);
      DefaultDataWriteUtility.Write(writer, "HeadDamage", headDamage);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      damageParameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "DamageParameterName");
      difficultyMultiplierParameterName = DefaultDataReadUtility.Read(reader, "DifficultyMultiplierParameterName", difficultyMultiplierParameterName);
      bodyDamage = DefaultDataReadUtility.Read(reader, "BodyDamage", bodyDamage);
      armDamage = DefaultDataReadUtility.Read(reader, "ArmDamage", armDamage);
      legDamage = DefaultDataReadUtility.Read(reader, "LegDamage", legDamage);
      headDamage = DefaultDataReadUtility.Read(reader, "HeadDamage", headDamage);
    }
  }
}
