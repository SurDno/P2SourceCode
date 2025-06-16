using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerJumpAbilityController))]
  public class PlayerJumpAbilityController_Generated : 
    PlayerJumpAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerJumpAbilityController_Generated instance = Activator.CreateInstance<PlayerJumpAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((PlayerJumpAbilityController) target2).jump = this.jump;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "jump", this.jump);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.jump = DefaultDataReadUtility.Read(reader, "jump", this.jump);
    }
  }
}
