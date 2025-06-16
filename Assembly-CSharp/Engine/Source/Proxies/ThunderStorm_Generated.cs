using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ThunderStorm))]
  public class ThunderStorm_Generated : 
    ThunderStorm,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ThunderStorm_Generated instance = Activator.CreateInstance<ThunderStorm_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ThunderStorm_Generated thunderStormGenerated = (ThunderStorm_Generated) target2;
      thunderStormGenerated.distanceFrom = distanceFrom;
      thunderStormGenerated.distanceTo = distanceTo;
      thunderStormGenerated.frequency = frequency;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "DistanceFrom", distanceFrom);
      DefaultDataWriteUtility.Write(writer, "DistanceTo", distanceTo);
      DefaultDataWriteUtility.Write(writer, "Frequency", frequency);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      distanceFrom = DefaultDataReadUtility.Read(reader, "DistanceFrom", distanceFrom);
      distanceTo = DefaultDataReadUtility.Read(reader, "DistanceTo", distanceTo);
      frequency = DefaultDataReadUtility.Read(reader, "Frequency", frequency);
    }
  }
}
