using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Repairing;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairableCostItem))]
  public class RepairableCostItem_Generated : 
    RepairableCostItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RepairableCostItem_Generated instance = Activator.CreateInstance<RepairableCostItem_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RepairableCostItem_Generated costItemGenerated = (RepairableCostItem_Generated) target2;
      costItemGenerated.template = this.template;
      costItemGenerated.count = this.count;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.template);
      DefaultDataWriteUtility.Write(writer, "Count", this.count);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.template);
      this.count = DefaultDataReadUtility.Read(reader, "Count", this.count);
    }
  }
}
