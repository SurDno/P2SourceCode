using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      FlameAbilityController_Generated controllerGenerated = (FlameAbilityController_Generated) target2;
      controllerGenerated.realTime = realTime;
      controllerGenerated.interval = interval;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
    }
  }
}
