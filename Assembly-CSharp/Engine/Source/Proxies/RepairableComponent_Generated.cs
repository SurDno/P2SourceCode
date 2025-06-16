using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((RepairableComponent) target2).settings = settings;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Settings", settings);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      settings = UnityDataReadUtility.Read(reader, "Settings", settings);
    }
  }
}
