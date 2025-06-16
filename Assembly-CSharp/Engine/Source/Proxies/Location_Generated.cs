using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      Location_Generated locationGenerated = (Location_Generated) target2;
      locationGenerated.latitude = latitude;
      locationGenerated.longitude = longitude;
      locationGenerated.utc = utc;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Latitude", latitude);
      DefaultDataWriteUtility.Write(writer, "Longitude", longitude);
      DefaultDataWriteUtility.Write(writer, "Utc", utc);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      latitude = DefaultDataReadUtility.Read(reader, "Latitude", latitude);
      longitude = DefaultDataReadUtility.Read(reader, "Longitude", longitude);
      utc = DefaultDataReadUtility.Read(reader, "Utc", utc);
    }
  }
}
