// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.IndoorCrowdComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Crowds;
using Engine.Source.Connections;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (IndoorCrowdComponent))]
  public class IndoorCrowdComponent_Generated : 
    IndoorCrowdComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      IndoorCrowdComponent_Generated instance = Activator.CreateInstance<IndoorCrowdComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      IndoorCrowdComponent_Generated componentGenerated = (IndoorCrowdComponent_Generated) target2;
      componentGenerated.region = CloneableObjectUtility.Clone<SceneGameObject>(this.region);
      CloneableObjectUtility.CopyListTo<CrowdAreaInfo>(componentGenerated.areas, this.areas);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<SceneGameObject>(writer, "Region", this.region);
      DefaultDataWriteUtility.WriteListSerialize<CrowdAreaInfo>(writer, "Areas", this.areas);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.region = DefaultDataReadUtility.ReadSerialize<SceneGameObject>(reader, "Region");
      this.areas = DefaultDataReadUtility.ReadListSerialize<CrowdAreaInfo>(reader, "Areas", this.areas);
    }
  }
}
