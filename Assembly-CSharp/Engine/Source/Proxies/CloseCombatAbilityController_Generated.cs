// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.CloseCombatAbilityController_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons.Abilities.Controllers;
using System;

#nullable disable
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloseCombatAbilityController_Generated controllerGenerated = (CloseCombatAbilityController_Generated) target2;
      controllerGenerated.punchType = this.punchType;
      controllerGenerated.punchSubtype = this.punchSubtype;
      controllerGenerated.weaponKind = this.weaponKind;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ShotType>(writer, "Punch", this.punchType);
      DefaultDataWriteUtility.WriteEnum<ShotSubtypeEnum>(writer, "PunchSubtype", this.punchSubtype);
      DefaultDataWriteUtility.WriteEnum<WeaponEnum>(writer, "weapon", this.weaponKind);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.punchType = DefaultDataReadUtility.ReadEnum<ShotType>(reader, "Punch");
      this.punchSubtype = DefaultDataReadUtility.ReadEnum<ShotSubtypeEnum>(reader, "PunchSubtype");
      this.weaponKind = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "weapon");
    }
  }
}
