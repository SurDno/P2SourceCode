// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SpawnpointComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Milestone;
using Engine.Source.Components;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SpawnpointComponent))]
  public class SpawnpointComponent_Generated : 
    SpawnpointComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SpawnpointComponent_Generated instance = Activator.CreateInstance<SpawnpointComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((SpawnpointComponent) target2).type = this.type;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<Kind>(writer, "Type", this.type);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.type = DefaultDataReadUtility.ReadEnum<Kind>(reader, "Type");
    }
  }
}
