using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairableComponent))]
  public class RepairableComponent_Generated : 
    RepairableComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RepairableComponent_Generated instance = Activator.CreateInstance<RepairableComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((RepairableComponent) target2).settings = this.settings;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<RepairableSettings>(writer, "Settings", this.settings);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.settings = UnityDataReadUtility.Read<RepairableSettings>(reader, "Settings", this.settings);
    }
  }
}
