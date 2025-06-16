using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Repairing;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      RepairableCostItem_Generated costItemGenerated = (RepairableCostItem_Generated) target2;
      costItemGenerated.template = template;
      costItemGenerated.count = count;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Template", template);
      DefaultDataWriteUtility.Write(writer, "Count", count);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      template = UnityDataReadUtility.Read(reader, "Template", template);
      count = DefaultDataReadUtility.Read(reader, "Count", count);
    }
  }
}
