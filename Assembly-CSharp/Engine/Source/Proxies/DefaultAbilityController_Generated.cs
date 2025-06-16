using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DefaultAbilityController))]
  public class DefaultAbilityController_Generated : 
    DefaultAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DefaultAbilityController_Generated instance = Activator.CreateInstance<DefaultAbilityController_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((DefaultAbilityController_Generated) target2).active = active;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Active", active);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      active = DefaultDataReadUtility.Read(reader, "Active", active);
    }
  }
}
