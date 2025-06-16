using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Selectors;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SelectorComponent))]
  public class SelectorComponent_Generated : 
    SelectorComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SelectorComponent_Generated instance = Activator.CreateInstance<SelectorComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<SelectorPreset>(((SelectorComponent) target2).presets, this.presets);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<SelectorPreset>(writer, "Presets", this.presets);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.presets = DefaultDataReadUtility.ReadListSerialize<SelectorPreset>(reader, "Presets", this.presets);
    }
  }
}
