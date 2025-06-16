using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerFallDamageAbilityController))]
  public class PlayerFallDamageAbilityController_Generated : 
    PlayerFallDamageAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerFallDamageAbilityController_Generated instance = Activator.CreateInstance<PlayerFallDamageAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      PlayerFallDamageAbilityController_Generated controllerGenerated = (PlayerFallDamageAbilityController_Generated) target2;
      controllerGenerated.minFall = this.minFall;
      controllerGenerated.maxFall = this.maxFall;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "minFall", this.minFall);
      DefaultDataWriteUtility.Write(writer, "maxFall", this.maxFall);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.minFall = DefaultDataReadUtility.Read(reader, "minFall", this.minFall);
      this.maxFall = DefaultDataReadUtility.Read(reader, "maxFall", this.maxFall);
    }
  }
}
