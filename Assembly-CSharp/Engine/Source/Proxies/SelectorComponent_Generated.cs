// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SelectorComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Selectors;
using System;

#nullable disable
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
