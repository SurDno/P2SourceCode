using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InterfaceBlockingService))]
  public class InterfaceBlockingService_Generated : 
    InterfaceBlockingService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MapInterfaceBlocked", this.mapInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "MindMapInterfaceBlocked", this.mindMapInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "InventoryInterfaceBlocked", this.inventoryInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "StatsInterfaceBlocked", this.statsInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "BoundsInterfaceBlocked", this.boundsInterfaceBlocked);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.mapInterfaceBlocked = DefaultDataReadUtility.Read(reader, "MapInterfaceBlocked", this.mapInterfaceBlocked);
      this.mindMapInterfaceBlocked = DefaultDataReadUtility.Read(reader, "MindMapInterfaceBlocked", this.mindMapInterfaceBlocked);
      this.inventoryInterfaceBlocked = DefaultDataReadUtility.Read(reader, "InventoryInterfaceBlocked", this.inventoryInterfaceBlocked);
      this.statsInterfaceBlocked = DefaultDataReadUtility.Read(reader, "StatsInterfaceBlocked", this.statsInterfaceBlocked);
      this.boundsInterfaceBlocked = DefaultDataReadUtility.Read(reader, "BoundsInterfaceBlocked", this.boundsInterfaceBlocked);
    }
  }
}
