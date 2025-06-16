// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SelectorPreset_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Selectors;
using Engine.Source.Connections;
using System;

#nullable disable
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
