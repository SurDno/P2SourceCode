using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.MindMap;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

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
