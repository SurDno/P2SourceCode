using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TimeService))]
  public class TimeService_Generated : TimeService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "AbsoluteGameTime", this.AbsoluteGameTime);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.RealTime);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.AbsoluteGameTime = DefaultDataReadUtility.Read(reader, "AbsoluteGameTime", this.AbsoluteGameTime);
      this.RealTime = DefaultDataReadUtility.Read(reader, "RealTime", this.RealTime);
    }
  }
}
