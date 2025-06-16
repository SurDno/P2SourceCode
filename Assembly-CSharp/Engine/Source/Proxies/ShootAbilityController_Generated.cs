using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ShootAbilityController))]
  public class ShootAbilityController_Generated : 
    ShootAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ShootAbilityController_Generated instance = Activator.CreateInstance<ShootAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ShootAbilityController_Generated controllerGenerated = (ShootAbilityController_Generated) target2;
      controllerGenerated.weaponKind = this.weaponKind;
      controllerGenerated.shot = this.shot;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<WeaponKind>(writer, "Weapon", this.weaponKind);
      DefaultDataWriteUtility.WriteEnum<ShotType>(writer, "Shot", this.shot);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.weaponKind = DefaultDataReadUtility.ReadEnum<WeaponKind>(reader, "Weapon");
      this.shot = DefaultDataReadUtility.ReadEnum<ShotType>(reader, "Shot");
    }
  }
}
