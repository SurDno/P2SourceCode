using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (HolsterAbilityController))]
  public class HolsterAbilityController_Generated : 
    HolsterAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      HolsterAbilityController_Generated instance = Activator.CreateInstance<HolsterAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((HolsterAbilityController) target2).weaponKind = this.weaponKind;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<WeaponKind>(writer, "Weapon", this.weaponKind);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.weaponKind = DefaultDataReadUtility.ReadEnum<WeaponKind>(reader, "Weapon");
    }
  }
}
