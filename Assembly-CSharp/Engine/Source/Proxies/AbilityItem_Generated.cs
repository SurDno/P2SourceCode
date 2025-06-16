using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      AbilityItem_Generated abilityItemGenerated = (AbilityItem_Generated) target2;
      abilityItemGenerated.name = name;
      abilityItemGenerated.controller = CloneableObjectUtility.Clone(controller);
      abilityItemGenerated.projectile = CloneableObjectUtility.Clone(projectile);
      abilityItemGenerated.targets = targets;
      CloneableObjectUtility.CopyListTo(abilityItemGenerated.effects, effects);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", name);
      DefaultDataWriteUtility.WriteSerialize(writer, "AbilityController", controller);
      DefaultDataWriteUtility.WriteSerialize(writer, "AbilityProjectile", projectile);
      DefaultDataWriteUtility.WriteEnum(writer, "Targets", targets);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Effects", effects);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.Read(reader, "Name", name);
      controller = DefaultDataReadUtility.ReadSerialize<IAbilityController>(reader, "AbilityController");
      projectile = DefaultDataReadUtility.ReadSerialize<IAbilityProjectile>(reader, "AbilityProjectile");
      targets = DefaultDataReadUtility.ReadEnum<AbilityTargetEnum>(reader, "Targets");
      effects = DefaultDataReadUtility.ReadListSerialize(reader, "Effects", effects);
    }
  }
}
