using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((DefaultAbilityController) target2).active = this.active;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Active", this.active);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.active = DefaultDataReadUtility.Read(reader, "Active", this.active);
    }
  }
}
