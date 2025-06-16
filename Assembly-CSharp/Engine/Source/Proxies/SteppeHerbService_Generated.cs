using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;
using System;

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
      DefaultDataWriteUtility.Write(writer, "BrownTwyreAmount", this.BrownTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "BloodTwyreAmount", this.BloodTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "BlackTwyreAmount", this.BlackTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "SweveryAmount", this.SweveryAmount);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.BrownTwyreAmount = DefaultDataReadUtility.Read(reader, "BrownTwyreAmount", this.BrownTwyreAmount);
      this.BloodTwyreAmount = DefaultDataReadUtility.Read(reader, "BloodTwyreAmount", this.BloodTwyreAmount);
      this.BlackTwyreAmount = DefaultDataReadUtility.Read(reader, "BlackTwyreAmount", this.BlackTwyreAmount);
      this.SweveryAmount = DefaultDataReadUtility.Read(reader, "SweveryAmount", this.SweveryAmount);
    }
  }
}
