using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons.Abilities.Controllers;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ShootAbilityController_Generated controllerGenerated = (ShootAbilityController_Generated) target2;
      controllerGenerated.weaponKind = weaponKind;
      controllerGenerated.shot = shot;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Weapon", weaponKind);
      DefaultDataWriteUtility.WriteEnum(writer, "Shot", shot);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      weaponKind = DefaultDataReadUtility.ReadEnum<WeaponKind>(reader, "Weapon");
      shot = DefaultDataReadUtility.ReadEnum<ShotType>(reader, "Shot");
    }
  }
}
