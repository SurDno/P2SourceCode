// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.MapTooltipResource_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MapTooltipResource))]
  public class MapTooltipResource_Generated : 
    MapTooltipResource,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<MapTooltipResource_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      MapTooltipResource_Generated resourceGenerated = (MapTooltipResource_Generated) target2;
      resourceGenerated.name = this.name;
      resourceGenerated.image = this.image;
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
