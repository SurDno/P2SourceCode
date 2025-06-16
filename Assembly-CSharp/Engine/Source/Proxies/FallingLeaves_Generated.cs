using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      FallingLeaves_Generated fallingLeavesGenerated = (FallingLeaves_Generated) target2;
      fallingLeavesGenerated.deviation = this.deviation;
      fallingLeavesGenerated.minLandingNormalY = this.minLandingNormalY;
      fallingLeavesGenerated.poolCapacity = this.poolCapacity;
      fallingLeavesGenerated.radius = this.radius;
      fallingLeavesGenerated.rate = this.rate;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Deviation", this.deviation);
      DefaultDataWriteUtility.Write(writer, "MinLandingNormalY", this.minLandingNormalY);
      DefaultDataWriteUtility.Write(writer, "PoolCapacity", this.poolCapacity);
      DefaultDataWriteUtility.Write(writer, "Radius", this.radius);
      DefaultDataWriteUtility.Write(writer, "Rate", this.rate);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.deviation = DefaultDataReadUtility.Read(reader, "Deviation", this.deviation);
      this.minLandingNormalY = DefaultDataReadUtility.Read(reader, "MinLandingNormalY", this.minLandingNormalY);
      this.poolCapacity = DefaultDataReadUtility.Read(reader, "PoolCapacity", this.poolCapacity);
      this.radius = DefaultDataReadUtility.Read(reader, "Radius", this.radius);
      this.rate = DefaultDataReadUtility.Read(reader, "Rate", this.rate);
    }
  }
}
