// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.MMPlaceholder_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.MindMap;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MMPlaceholder))]
  public class MMPlaceholder_Generated : 
    MMPlaceholder,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<MMPlaceholder_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      MMPlaceholder_Generated placeholderGenerated = (MMPlaceholder_Generated) target2;
      placeholderGenerated.name = this.name;
      placeholderGenerated.image = this.image;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      UnityDataWriteUtility.Write<Texture>(writer, "Image", this.image);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.image = UnityDataReadUtility.Read<Texture>(reader, "Image", this.image);
    }
  }
}
