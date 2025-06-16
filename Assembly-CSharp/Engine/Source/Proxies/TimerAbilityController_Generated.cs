using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TimerAbilityController))]
  public class TimerAbilityController_Generated : 
    TimerAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      TimerAbilityController_Generated instance = Activator.CreateInstance<TimerAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      TimerAbilityController_Generated controllerGenerated = (TimerAbilityController_Generated) target2;
      controllerGenerated.realTime = this.realTime;
      controllerGenerated.interval = this.interval;
      controllerGenerated.timeout = this.timeout;
      controllerGenerated.intervalTime = this.intervalTime;
      controllerGenerated.timeoutTime = this.timeoutTime;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.Write(writer, "Timeout", this.timeout);
      DefaultDataWriteUtility.Write(writer, "IntervalTime", this.intervalTime);
      DefaultDataWriteUtility.Write(writer, "TimeoutTime", this.timeoutTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.timeout = DefaultDataReadUtility.Read(reader, "Timeout", this.timeout);
      this.intervalTime = DefaultDataReadUtility.Read(reader, "IntervalTime", this.intervalTime);
      this.timeoutTime = DefaultDataReadUtility.Read(reader, "TimeoutTime", this.timeoutTime);
    }
  }
}
