// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.WeatherSnapshot_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.Weather;
using Engine.Impl.Weather.Element;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherSnapshot))]
  public class WeatherSnapshot_Generated : 
    WeatherSnapshot,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<WeatherSnapshot_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      WeatherSnapshot_Generated snapshotGenerated = (WeatherSnapshot_Generated) target2;
      snapshotGenerated.name = this.name;
      ((ICopyable) this.clouds).CopyTo((object) snapshotGenerated.clouds);
      ((ICopyable) this.day).CopyTo((object) snapshotGenerated.day);
      ((ICopyable) this.fog).CopyTo((object) snapshotGenerated.fog);
      ((ICopyable) this.location).CopyTo((object) snapshotGenerated.location);
      ((ICopyable) this.moon).CopyTo((object) snapshotGenerated.moon);
      ((ICopyable) this.night).CopyTo((object) snapshotGenerated.night);
      ((ICopyable) this.stars).CopyTo((object) snapshotGenerated.stars);
      ((ICopyable) this.sun).CopyTo((object) snapshotGenerated.sun);
      ((ICopyable) this.thunderStorm).CopyTo((object) snapshotGenerated.thunderStorm);
      ((ICopyable) this.wind).CopyTo((object) snapshotGenerated.wind);
      ((ICopyable) this.rain).CopyTo((object) snapshotGenerated.rain);
      ((ICopyable) this.fallingLeaves).CopyTo((object) snapshotGenerated.fallingLeaves);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteSerialize<Clouds>(writer, "Clouds", this.clouds);
      DefaultDataWriteUtility.WriteSerialize<Day>(writer, "Day", this.day);
      DefaultDataWriteUtility.WriteSerialize<Fog>(writer, "Fog", this.fog);
      DefaultDataWriteUtility.WriteSerialize<Location>(writer, "Location", this.location);
      DefaultDataWriteUtility.WriteSerialize<Moon>(writer, "Moon", this.moon);
      DefaultDataWriteUtility.WriteSerialize<Night>(writer, "Night", this.night);
      DefaultDataWriteUtility.WriteSerialize<Stars>(writer, "Stars", this.stars);
      DefaultDataWriteUtility.WriteSerialize<Sun>(writer, "Sun", this.sun);
      DefaultDataWriteUtility.WriteSerialize<ThunderStorm>(writer, "ThunderStorm", this.thunderStorm);
      DefaultDataWriteUtility.WriteSerialize<Wind>(writer, "Wind", this.wind);
      DefaultDataWriteUtility.WriteSerialize<Rain>(writer, "Rain", this.rain);
      DefaultDataWriteUtility.WriteSerialize<FallingLeaves>(writer, "FallingLeaves", this.fallingLeaves);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      IDataReader child1 = reader.GetChild("Clouds");
      Clouds clouds = this.clouds;
      if (clouds is ISerializeDataRead serializeDataRead1)
        serializeDataRead1.DataRead(child1, typeof (Clouds));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(clouds.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child2 = reader.GetChild("Day");
      Day day = this.day;
      if (day is ISerializeDataRead serializeDataRead2)
        serializeDataRead2.DataRead(child2, typeof (Day));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(day.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child3 = reader.GetChild("Fog");
      Fog fog = this.fog;
      if (fog is ISerializeDataRead serializeDataRead3)
        serializeDataRead3.DataRead(child3, typeof (Fog));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(fog.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child4 = reader.GetChild("Location");
      Location location = this.location;
      if (location is ISerializeDataRead serializeDataRead4)
        serializeDataRead4.DataRead(child4, typeof (Location));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(location.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child5 = reader.GetChild("Moon");
      Moon moon = this.moon;
      if (moon is ISerializeDataRead serializeDataRead5)
        serializeDataRead5.DataRead(child5, typeof (Moon));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(moon.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child6 = reader.GetChild("Night");
      Night night = this.night;
      if (night is ISerializeDataRead serializeDataRead6)
        serializeDataRead6.DataRead(child6, typeof (Night));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(night.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child7 = reader.GetChild("Stars");
      Stars stars = this.stars;
      if (stars is ISerializeDataRead serializeDataRead7)
        serializeDataRead7.DataRead(child7, typeof (Stars));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(stars.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child8 = reader.GetChild("Sun");
      Sun sun = this.sun;
      if (sun is ISerializeDataRead serializeDataRead8)
        serializeDataRead8.DataRead(child8, typeof (Sun));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(sun.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child9 = reader.GetChild("ThunderStorm");
      ThunderStorm thunderStorm = this.thunderStorm;
      if (thunderStorm is ISerializeDataRead serializeDataRead9)
        serializeDataRead9.DataRead(child9, typeof (ThunderStorm));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(thunderStorm.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child10 = reader.GetChild("Wind");
      Wind wind = this.wind;
      if (wind is ISerializeDataRead serializeDataRead10)
        serializeDataRead10.DataRead(child10, typeof (Wind));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(wind.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child11 = reader.GetChild("Rain");
      Rain rain = this.rain;
      if (rain is ISerializeDataRead serializeDataRead11)
        serializeDataRead11.DataRead(child11, typeof (Rain));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(rain.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child12 = reader.GetChild("FallingLeaves");
      FallingLeaves fallingLeaves = this.fallingLeaves;
      if (fallingLeaves is ISerializeDataRead serializeDataRead12)
        serializeDataRead12.DataRead(child12, typeof (FallingLeaves));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(fallingLeaves.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
    }
  }
}
