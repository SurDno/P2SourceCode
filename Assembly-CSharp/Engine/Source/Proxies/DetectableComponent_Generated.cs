using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DetectableComponent))]
  public class DetectableComponent_Generated : 
    DetectableComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      DetectableComponent_Generated instance = Activator.CreateInstance<DetectableComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((DetectableComponent) target2).isEnabled = this.isEnabled;
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
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }
  }
}
