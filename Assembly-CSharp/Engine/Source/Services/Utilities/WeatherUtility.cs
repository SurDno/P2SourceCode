using System;
using Engine.Common.Services;
using Engine.Common.Weather;

namespace Engine.Source.Services.Utilities
{
  public static class WeatherUtility
  {
    public static void SetDefaultWeather(IWeatherController weather, IWeatherSnapshot snapshot)
    {
      IWeatherLayerBlenderItem layerBlenderItem1 = weather.GetItem(WeatherLayer.BaseLayer);
      if (layerBlenderItem1 != null)
      {
        layerBlenderItem1.SetOpacity(1f);
        if (snapshot != null)
          layerBlenderItem1.Blender.BlendTo(snapshot, TimeSpan.Zero);
      }
      IWeatherLayerBlenderItem layerBlenderItem2 = weather.GetItem(WeatherLayer.CutSceneLayer);
      if (layerBlenderItem2 != null)
      {
        layerBlenderItem2.SetOpacity(0.0f);
        if (snapshot != null)
          layerBlenderItem2.Blender.BlendTo(snapshot, TimeSpan.Zero);
      }
      IWeatherLayerBlenderItem layerBlenderItem3 = weather.GetItem(WeatherLayer.DistrictLayer);
      if (layerBlenderItem3 != null)
      {
        layerBlenderItem3.SetOpacity(0.0f);
        if (snapshot != null)
          layerBlenderItem3.Blender.BlendTo(snapshot, TimeSpan.Zero);
      }
      IWeatherLayerBlenderItem layerBlenderItem4 = weather.GetItem(WeatherLayer.PlannedEventsLayer);
      if (layerBlenderItem4 == null)
        return;
      layerBlenderItem4.SetOpacity(0.0f);
      if (snapshot != null)
        layerBlenderItem4.Blender.BlendTo(snapshot, TimeSpan.Zero);
    }
  }
}
