using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FlameAbilityController))]
  public class FlameAbilityController_Generated : 
    FlameAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FlameAbilityController_Generated instance = Activator.CreateInstance<FlameAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      FlameAbilityController_Generated controllerGenerated = (FlameAbilityController_Generated) target2;
      controllerGenerated.realTime = this.realTime;
      controllerGenerated.interval = this.interval;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
    }
  }
}
