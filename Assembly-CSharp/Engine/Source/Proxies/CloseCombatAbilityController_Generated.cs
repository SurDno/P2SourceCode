using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CloseCombatAbilityController))]
  public class CloseCombatAbilityController_Generated : 
    CloseCombatAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CloseCombatAbilityController_Generated instance = Activator.CreateInstance<CloseCombatAbilityController_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloseCombatAbilityController_Generated controllerGenerated = (CloseCombatAbilityController_Generated) target2;
      controllerGenerated.punchType = punchType;
      controllerGenerated.punchSubtype = punchSubtype;
      controllerGenerated.weaponKind = weaponKind;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Punch", punchType);
      DefaultDataWriteUtility.WriteEnum(writer, "PunchSubtype", punchSubtype);
      DefaultDataWriteUtility.WriteEnum(writer, "weapon", weaponKind);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      punchType = DefaultDataReadUtility.ReadEnum<ShotType>(reader, "Punch");
      punchSubtype = DefaultDataReadUtility.ReadEnum<ShotSubtypeEnum>(reader, "PunchSubtype");
      weaponKind = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "weapon");
    }
  }
}
