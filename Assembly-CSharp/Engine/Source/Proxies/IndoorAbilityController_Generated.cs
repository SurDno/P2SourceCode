using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (IndoorAbilityController))]
  public class IndoorAbilityController_Generated : 
    IndoorAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      IndoorAbilityController_Generated instance = Activator.CreateInstance<IndoorAbilityController_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
    }

    public void DataWrite(IDataWriter writer)
    {
    }

    public void DataRead(IDataReader reader, Type type)
    {
    }
  }
}
