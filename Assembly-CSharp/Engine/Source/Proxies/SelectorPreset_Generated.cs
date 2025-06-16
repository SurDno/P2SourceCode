using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Selectors;
using Engine.Source.Connections;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SelectorPreset))]
  public class SelectorPreset_Generated : 
    SelectorPreset,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SelectorPreset_Generated instance = Activator.CreateInstance<SelectorPreset_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<SceneGameObject>(((SelectorPreset) target2).Objects, this.Objects);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<SceneGameObject>(writer, "Objects", this.Objects);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Objects = DefaultDataReadUtility.ReadListSerialize<SceneGameObject>(reader, "Objects", this.Objects);
    }
  }
}
