using Cofe.Meta;
using Engine.Common.Blenders;
using Engine.Common.Services;
using Engine.Common.Weather;
using Engine.Impl.Services;
using Engine.Source.Blenders;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class WeatherGroupDebug
  {
    private static string name = "[Weather]";
    private static KeyCode key = KeyCode.W;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = ColorPreset.Orange;
    private static Color bodyColor = Color.white;

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(WeatherGroupDebug.name, WeatherGroupDebug.key, WeatherGroupDebug.modifficators, new Action(WeatherGroupDebug.Update)));
    }

    private static void Update()
    {
      WeatherController service1 = ServiceLocator.GetService<WeatherController>();
      if (InputUtility.IsKeyDown(KeyCode.KeypadDivide))
        service1.IsEnabled = !service1.IsEnabled;
      if (InputUtility.IsKeyDown(KeyCode.KeypadPeriod))
      {
        IWeatherSnapshot weatherSnapshot = ServiceLocator.GetService<ITemplateService>().GetTemplates<IWeatherSnapshot>().Random<IWeatherSnapshot>();
        if (weatherSnapshot != null && service1.IsEnabled)
        {
          ILayerBlenderItem<IWeatherSnapshot> layerBlenderItem = service1.Blender.Items.FirstOrDefault<ILayerBlenderItem<IWeatherSnapshot>>();
          if (layerBlenderItem != null)
          {
            ITimeService service2 = ServiceLocator.GetService<ITimeService>();
            layerBlenderItem.Blender.BlendTo(weatherSnapshot, TimeSpan.FromSeconds(1.0 * (double) service2.GameTimeFactor));
            Debug.Log((object) ObjectInfoUtility.GetStream().Append("Change weather to : [").Append(weatherSnapshot.Name).Append("] : [").Append(weatherSnapshot.Source).Append("]"));
          }
        }
      }
      string text1 = "\n" + WeatherGroupDebug.name + " (" + InputUtility.GetHotKeyText(WeatherGroupDebug.key, WeatherGroupDebug.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, WeatherGroupDebug.headerColor);
      string text2 = "  Enabled : " + service1.IsEnabled.ToString();
      int num = 0;
      foreach (ILayerBlenderItem<IWeatherSnapshot> layerBlenderItem in service1.Blender.Items)
      {
        text2 = text2 + "\n  Weather Layer " + (object) (WeatherLayer) num + " : ";
        WeatherSmoothBlender blender = (WeatherSmoothBlender) layerBlenderItem.Blender;
        text2 = text2 + "[" + blender.Current.Name + "]";
        if (blender.Target != null)
          text2 = text2 + " > [" + blender.Target.Name + "] , Progress : " + blender.Progress.ToString("F2");
        text2 = text2 + " , Opacity : " + (object) layerBlenderItem.Opacity;
        ++num;
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text2, WeatherGroupDebug.bodyColor);
    }
  }
}
