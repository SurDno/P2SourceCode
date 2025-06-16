// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Location_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Location))]
  public class Location_Generated : 
    Location,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      Location_Generated instance = Activator.CreateInstance<Location_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Location_Generated locationGenerated = (Location_Generated) target2;
      locationGenerated.latitude = this.latitude;
      locationGenerated.longitude = this.longitude;
      locationGenerated.utc = this.utc;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Latitude", this.latitude);
      DefaultDataWriteUtility.Write(writer, "Longitude", this.longitude);
      DefaultDataWriteUtility.Write(writer, "Utc", this.utc);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.latitude = DefaultDataReadUtility.Read(reader, "Latitude", this.latitude);
      this.longitude = DefaultDataReadUtility.Read(reader, "Longitude", this.longitude);
      this.utc = DefaultDataReadUtility.Read(reader, "Utc", this.utc);
    }
  }
}
