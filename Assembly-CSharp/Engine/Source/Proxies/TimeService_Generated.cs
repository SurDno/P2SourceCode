using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TimeService))]
  public class TimeService_Generated : TimeService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "AbsoluteGameTime", AbsoluteGameTime);
      DefaultDataWriteUtility.Write(writer, "RealTime", RealTime);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      AbsoluteGameTime = DefaultDataReadUtility.Read(reader, "AbsoluteGameTime", AbsoluteGameTime);
      RealTime = DefaultDataReadUtility.Read(reader, "RealTime", RealTime);
    }
  }
}
