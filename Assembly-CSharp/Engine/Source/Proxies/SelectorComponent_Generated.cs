using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo(((SelectorComponent) target2).presets, presets);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize(writer, "Presets", presets);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      presets = DefaultDataReadUtility.ReadListSerialize(reader, "Presets", presets);
    }
  }
}
