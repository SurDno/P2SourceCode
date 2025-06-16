using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Repairing;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RepairableLevel_Generated repairableLevelGenerated = (RepairableLevel_Generated) target2;
      repairableLevelGenerated.maxDurability = this.maxDurability;
      CloneableObjectUtility.CopyListTo<RepairableCostItem>(repairableLevelGenerated.cost, this.cost);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MaxDurability", this.maxDurability);
      DefaultDataWriteUtility.WriteListSerialize<RepairableCostItem>(writer, "Cost", this.cost);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.maxDurability = DefaultDataReadUtility.Read(reader, "MaxDurability", this.maxDurability);
      this.cost = DefaultDataReadUtility.ReadListSerialize<RepairableCostItem>(reader, "Cost", this.cost);
    }
  }
}
