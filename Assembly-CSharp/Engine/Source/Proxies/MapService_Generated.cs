using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MapService))]
  public class MapService_Generated : MapService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "BullModeAvailable", this.BullModeAvailable);
      DefaultDataWriteUtility.Write(writer, "BullModeForced", this.BullModeForced);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.BullModeAvailable = DefaultDataReadUtility.Read(reader, "BullModeAvailable", this.BullModeAvailable);
      this.BullModeForced = DefaultDataReadUtility.Read(reader, "BullModeForced", this.BullModeForced);
    }
  }
}
