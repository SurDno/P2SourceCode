// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.WeatherSnapshotUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Weather;
using Engine.Proxy.Weather.Element;
using Engine.Source.Services;
using System;

#nullable disable
namespace Engine.Proxy.Weather
{
  public static class WeatherSnapshotUtility
  {
    public static void CopyTo(WeatherSnapshot snapshot)
    {
      if (snapshot == null)
        throw new Exception();
      CloudsUtility.CopyTo(snapshot.Clouds);
      DayUtility.CopyTo(snapshot.Day);
      ServiceLocator.GetService<FogController>().CopyTo(snapshot.Fog);
      LocationUtility.CopyTo(snapshot.Location);
      MoonUtility.CopyTo(snapshot.Moon);
      NightUtility.CopyTo(snapshot.Night);
      StarsUtility.CopyTo(snapshot.Stars);
      SunUtility.CopyTo(snapshot.Sun);
      ThunderStormUtility.CopyTo(snapshot.ThunderStorm);
      WindUtility.CopyTo(snapshot.Wind);
      RainUtility.CopyTo(snapshot.Rain);
      FallingLeavesUtility.CopyTo(snapshot.FallingLeaves);
    }

    public static void CopyFrom(WeatherSnapshot snapshot)
    {
      if (snapshot == null)
        throw new Exception();
      CloudsUtility.CopyFrom(snapshot.Clouds);
      DayUtility.CopyFrom(snapshot.Day);
      ServiceLocator.GetService<FogController>().CopyFrom(snapshot.Fog);
      LocationUtility.CopyFrom(snapshot.Location);
      MoonUtility.CopyFrom(snapshot.Moon);
      NightUtility.CopyFrom(snapshot.Night);
      StarsUtility.CopyFrom(snapshot.Stars);
      SunUtility.CopyFrom(snapshot.Sun);
      ThunderStormUtility.CopyFrom(snapshot.ThunderStorm);
      WindUtility.CopyFrom(snapshot.Wind);
      RainUtility.CopyFrom(snapshot.Rain);
      FallingLeavesUtility.CopyFrom(snapshot.FallingLeaves);
    }
  }
}
