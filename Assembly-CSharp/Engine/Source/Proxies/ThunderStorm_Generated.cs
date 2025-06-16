// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ThunderStorm_Generated
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ThunderStorm_Generated thunderStormGenerated = (ThunderStorm_Generated) target2;
      thunderStormGenerated.distanceFrom = this.distanceFrom;
      thunderStormGenerated.distanceTo = this.distanceTo;
      thunderStormGenerated.frequency = this.frequency;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "DistanceFrom", this.distanceFrom);
      DefaultDataWriteUtility.Write(writer, "DistanceTo", this.distanceTo);
      DefaultDataWriteUtility.Write(writer, "Frequency", this.frequency);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.distanceFrom = DefaultDataReadUtility.Read(reader, "DistanceFrom", this.distanceFrom);
      this.distanceTo = DefaultDataReadUtility.Read(reader, "DistanceTo", this.distanceTo);
      this.frequency = DefaultDataReadUtility.Read(reader, "Frequency", this.frequency);
    }
  }
}
