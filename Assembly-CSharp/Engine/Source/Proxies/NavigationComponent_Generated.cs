using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NavigationComponent))]
  public class NavigationComponent_Generated : 
    NavigationComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      NavigationComponent_Generated instance = Activator.CreateInstance<NavigationComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((NavigationComponent) target2).isEnabled = this.isEnabled;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveSerialize<TeleportData>(writer, "TeleportData", this.TeleportData);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      CustomStateSaveUtility.SaveReference(writer, "SetupPoint", (object) this.setupPoint);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.TeleportData = DefaultStateLoadUtility.ReadSerialize<TeleportData>(reader, "TeleportData");
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.setupPoint = CustomStateLoadUtility.LoadReference<IEntity>(reader, "SetupPoint", this.setupPoint);
    }
  }
}
