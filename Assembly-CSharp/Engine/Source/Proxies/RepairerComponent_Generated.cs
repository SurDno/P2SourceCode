using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Storable;
using Engine.Source.Components;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.FillListTo<StorableGroup>(((RepairerComponent) target2).repairableGroups, this.repairableGroups);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListEnum<StorableGroup>(writer, "RepairableGroups", this.repairableGroups);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.repairableGroups = DefaultDataReadUtility.ReadListEnum<StorableGroup>(reader, "RepairableGroups", this.repairableGroups);
    }
  }
}
