using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FallingLeaves))]
  public class FallingLeaves_Generated : 
    FallingLeaves,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FallingLeaves_Generated instance = Activator.CreateInstance<FallingLeaves_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      FallingLeaves_Generated fallingLeavesGenerated = (FallingLeaves_Generated) target2;
      fallingLeavesGenerated.deviation = deviation;
      fallingLeavesGenerated.minLandingNormalY = minLandingNormalY;
      fallingLeavesGenerated.poolCapacity = poolCapacity;
      fallingLeavesGenerated.radius = radius;
      fallingLeavesGenerated.rate = rate;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Deviation", deviation);
      DefaultDataWriteUtility.Write(writer, "MinLandingNormalY", minLandingNormalY);
      DefaultDataWriteUtility.Write(writer, "PoolCapacity", poolCapacity);
      DefaultDataWriteUtility.Write(writer, "Radius", radius);
      DefaultDataWriteUtility.Write(writer, "Rate", rate);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      deviation = DefaultDataReadUtility.Read(reader, "Deviation", deviation);
      minLandingNormalY = DefaultDataReadUtility.Read(reader, "MinLandingNormalY", minLandingNormalY);
      poolCapacity = DefaultDataReadUtility.Read(reader, "PoolCapacity", poolCapacity);
      radius = DefaultDataReadUtility.Read(reader, "Radius", radius);
      rate = DefaultDataReadUtility.Read(reader, "Rate", rate);
    }
  }
}
