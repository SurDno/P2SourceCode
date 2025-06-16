using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairerComponent))]
  public class RepairerComponent_Generated : 
    RepairerComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RepairerComponent_Generated instance = Activator.CreateInstance<RepairerComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.FillListTo(((RepairerComponent) target2).repairableGroups, repairableGroups);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum(writer, "RepairableGroups", repairableGroups);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      repairableGroups = DefaultDataReadUtility.ReadListEnum(reader, "RepairableGroups", repairableGroups);
    }
  }
}
