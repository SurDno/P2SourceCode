// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.FallingLeaves_Generated
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
