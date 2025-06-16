using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Locations;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LocationComponent))]
  public class LocationComponent_Generated : 
    LocationComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LocationComponent_Generated instance = Activator.CreateInstance<LocationComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ((LocationComponent) target2).locationType = locationType;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "LocationType", locationType);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      locationType = DefaultDataReadUtility.ReadEnum<LocationType>(reader, "LocationType");
    }
  }
}
