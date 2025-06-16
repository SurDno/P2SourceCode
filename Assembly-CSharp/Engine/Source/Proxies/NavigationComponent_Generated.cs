using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((NavigationComponent_Generated) target2).isEnabled = isEnabled;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveSerialize(writer, "TeleportData", TeleportData);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
      CustomStateSaveUtility.SaveReference(writer, "SetupPoint", setupPoint);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      TeleportData = DefaultStateLoadUtility.ReadSerialize<TeleportData>(reader, "TeleportData");
      isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
      setupPoint = CustomStateLoadUtility.LoadReference(reader, "SetupPoint", setupPoint);
    }
  }
}
