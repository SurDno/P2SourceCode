using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BehaviorTreeAbilityController))]
  public class BehaviorTreeAbilityController_Generated : 
    BehaviorTreeAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      BehaviorTreeAbilityController_Generated instance = Activator.CreateInstance<BehaviorTreeAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((BehaviorTreeAbilityController) target2).name = this.name;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
    }
  }
}
