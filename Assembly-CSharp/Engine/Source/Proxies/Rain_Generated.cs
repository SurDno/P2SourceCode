// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Rain_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Rain))]
  public class Rain_Generated : Rain, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead
  {
    public object Clone()
    {
      Rain_Generated instance = Activator.CreateInstance<Rain_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Rain_Generated rainGenerated = (Rain_Generated) target2;
      rainGenerated.direction = this.direction;
      rainGenerated.intensity = this.intensity;
      rainGenerated.puddlesDryTime = this.puddlesDryTime;
      rainGenerated.puddlesFillTime = this.puddlesFillTime;
      rainGenerated.terrainDryTime = this.terrainDryTime;
      rainGenerated.terrainFillTime = this.terrainFillTime;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Direction", this.direction);
      DefaultDataWriteUtility.Write(writer, "Intensity", this.intensity);
      DefaultDataWriteUtility.Write(writer, "PuddlesDryTime", this.puddlesDryTime);
      DefaultDataWriteUtility.Write(writer, "PuddlesFillTime", this.puddlesFillTime);
      DefaultDataWriteUtility.Write(writer, "TerrainDryTime", this.terrainDryTime);
      DefaultDataWriteUtility.Write(writer, "TerrainFillTime", this.terrainFillTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.direction = UnityDataReadUtility.Read(reader, "Direction", this.direction);
      this.intensity = DefaultDataReadUtility.Read(reader, "Intensity", this.intensity);
      this.puddlesDryTime = DefaultDataReadUtility.Read(reader, "PuddlesDryTime", this.puddlesDryTime);
      this.puddlesFillTime = DefaultDataReadUtility.Read(reader, "PuddlesFillTime", this.puddlesFillTime);
      this.terrainDryTime = DefaultDataReadUtility.Read(reader, "TerrainDryTime", this.terrainDryTime);
      this.terrainFillTime = DefaultDataReadUtility.Read(reader, "TerrainFillTime", this.terrainFillTime);
    }
  }
}
