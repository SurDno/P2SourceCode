using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Repairing;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairableLevel))]
  public class RepairableLevel_Generated : 
    RepairableLevel,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RepairableLevel_Generated instance = Activator.CreateInstance<RepairableLevel_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      RepairableLevel_Generated repairableLevelGenerated = (RepairableLevel_Generated) target2;
      repairableLevelGenerated.maxDurability = maxDurability;
      CloneableObjectUtility.CopyListTo(repairableLevelGenerated.cost, cost);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MaxDurability", maxDurability);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Cost", cost);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      maxDurability = DefaultDataReadUtility.Read(reader, "MaxDurability", maxDurability);
      cost = DefaultDataReadUtility.ReadListSerialize(reader, "Cost", cost);
    }
  }
}
