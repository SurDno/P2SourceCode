using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;

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
      DefaultDataWriteUtility.Write(writer, "MapInterfaceBlocked", mapInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "MindMapInterfaceBlocked", mindMapInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "InventoryInterfaceBlocked", inventoryInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "StatsInterfaceBlocked", statsInterfaceBlocked);
      DefaultDataWriteUtility.Write(writer, "BoundsInterfaceBlocked", boundsInterfaceBlocked);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      mapInterfaceBlocked = DefaultDataReadUtility.Read(reader, "MapInterfaceBlocked", mapInterfaceBlocked);
      mindMapInterfaceBlocked = DefaultDataReadUtility.Read(reader, "MindMapInterfaceBlocked", mindMapInterfaceBlocked);
      inventoryInterfaceBlocked = DefaultDataReadUtility.Read(reader, "InventoryInterfaceBlocked", inventoryInterfaceBlocked);
      statsInterfaceBlocked = DefaultDataReadUtility.Read(reader, "StatsInterfaceBlocked", statsInterfaceBlocked);
      boundsInterfaceBlocked = DefaultDataReadUtility.Read(reader, "BoundsInterfaceBlocked", boundsInterfaceBlocked);
    }
  }
}
