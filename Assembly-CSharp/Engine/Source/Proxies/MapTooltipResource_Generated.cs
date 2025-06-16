using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

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
