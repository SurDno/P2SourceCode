// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SceneObject_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Assets.Internal.Reference;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl;
using Engine.Source.Commons;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SceneObject))]
  public class SceneObject_Generated : 
    SceneObject,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone() => (object) ServiceCache.Factory.Instantiate<SceneObject_Generated>(this);

    public void CopyTo(object target2) => ((EngineObject) target2).name = this.name;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<SceneObjectItem>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.Items = DefaultDataReadUtility.ReadListSerialize<SceneObjectItem>(reader, "Items", this.Items);
    }
  }
}
