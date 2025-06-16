using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((PlayerJumpAbilityController) target2).jump = jump;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "jump", jump);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      jump = DefaultDataReadUtility.Read(reader, "jump", jump);
    }
  }
}
