using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Interactable;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InteractableComponent))]
  public class InteractableComponent_Generated : 
    InteractableComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      InteractableComponent_Generated instance = Activator.CreateInstance<InteractableComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      InteractableComponent_Generated componentGenerated = (InteractableComponent_Generated) target2;
      componentGenerated.isEnabled = this.isEnabled;
      CloneableObjectUtility.CopyListTo<InteractItem>(componentGenerated.items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      DefaultDataWriteUtility.WriteListSerialize<InteractItem>(writer, "Items", this.items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.items = DefaultDataReadUtility.ReadListSerialize<InteractItem>(reader, "Items", this.items);
    }

    public void StateSave(IDataWriter writer)
    {
      EngineDataWriteUtility.Write(writer, "Title", this.Title);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Title = EngineDataReadUtility.Read(reader, "Title", this.Title);
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
    }
  }
}
