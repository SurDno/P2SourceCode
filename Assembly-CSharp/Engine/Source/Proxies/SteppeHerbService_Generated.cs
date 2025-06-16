using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SteppeHerbService))]
  public class SteppeHerbService_Generated : 
    SteppeHerbService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "BrownTwyreAmount", BrownTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "BloodTwyreAmount", BloodTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "BlackTwyreAmount", BlackTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "SweveryAmount", SweveryAmount);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      BrownTwyreAmount = DefaultDataReadUtility.Read(reader, "BrownTwyreAmount", BrownTwyreAmount);
      BloodTwyreAmount = DefaultDataReadUtility.Read(reader, "BloodTwyreAmount", BloodTwyreAmount);
      BlackTwyreAmount = DefaultDataReadUtility.Read(reader, "BlackTwyreAmount", BlackTwyreAmount);
      SweveryAmount = DefaultDataReadUtility.Read(reader, "SweveryAmount", SweveryAmount);
    }
  }
}
