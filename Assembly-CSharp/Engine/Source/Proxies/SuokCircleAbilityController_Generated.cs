using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SuokCircleAbilityController))]
  public class SuokCircleAbilityController_Generated : 
    SuokCircleAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SuokCircleAbilityController_Generated instance = Activator.CreateInstance<SuokCircleAbilityController_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((SuokCircleAbilityController_Generated) target2).state = state;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "State", state);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      state = DefaultDataReadUtility.ReadEnum<SuokCircleTutorialStateEnum>(reader, "State");
    }
  }
}
