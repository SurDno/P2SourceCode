using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ItemUseAbilityController))]
  public class ItemUseAbilityController_Generated : 
    ItemUseAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ItemUseAbilityController_Generated instance = Activator.CreateInstance<ItemUseAbilityController_Generated>();
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
