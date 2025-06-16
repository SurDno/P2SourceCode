// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Weather
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Proxies;
using Engine.Common.Services;
using Engine.Common.Weather;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMWeather))]
  public class Weather : VMWeather, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Weather);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void Initialize(VMBaseEntity parent)
    {
      base.Initialize(parent);
      if (PLVirtualMachine.Dynamic.Components.Weather.Instance == null)
        PLVirtualMachine.Dynamic.Components.Weather.Instance = (VMWeather) this;
      else
        Logger.AddError("Weather component creation dublicate!");
    }

    public static void ResetInstance() => PLVirtualMachine.Dynamic.Components.Weather.Instance = (VMWeather) null;

    public override void SetWeatherLayerWeight(
      WeatherLayer weatherLayer,
      float fWeight,
      float fTimeToBlend)
    {
      IWeatherLayerBlenderItem layerBlenderItem = ServiceLocator.GetService<IWeatherController>().GetItem(weatherLayer);
      if (layerBlenderItem == null)
        Logger.AddError(string.Format("Invalid weather layer : {0}", (object) weatherLayer));
      else
        layerBlenderItem.SetOpacity(fWeight, TimeSpan.FromSeconds((double) fTimeToBlend));
    }

    public override void SetWeatherLayerWeightGT(
      WeatherLayer weatherLayer,
      float fWeight,
      GameTime timeToBlend)
    {
      IWeatherLayerBlenderItem layerBlenderItem = ServiceLocator.GetService<IWeatherController>().GetItem(weatherLayer);
      if (layerBlenderItem == null)
        Logger.AddError(string.Format("Invalid weather layer : {0}", (object) weatherLayer));
      else
        layerBlenderItem.SetOpacity(fWeight, TimeSpan.FromSeconds((double) timeToBlend.TotalSeconds));
    }

    public override void SetWeatherSample(
      WeatherLayer weatherLayer,
      ISampleRef weatherSample,
      float fTimeToBlend)
    {
      IWeatherLayerBlenderItem layerBlenderItem = ServiceLocator.GetService<IWeatherController>().GetItem(weatherLayer);
      if (layerBlenderItem == null)
      {
        Logger.AddError(string.Format("Invalid weather layer : {0}", (object) weatherLayer));
      }
      else
      {
        IWeatherSnapshot template = ServiceCache.TemplateService.GetTemplate<IWeatherSnapshot>(weatherSample.EngineTemplateGuid);
        if (template == null)
        {
          Logger.AddError(string.Format("Weather snapshot with guid={0} not found!", (object) weatherSample.EngineTemplateGuid));
        }
        else
        {
          ITimeService service = ServiceLocator.GetService<ITimeService>();
          layerBlenderItem.Blender.BlendTo(template, TimeSpan.FromSeconds((double) fTimeToBlend * (double) service.GameTimeFactor));
        }
      }
    }

    public override void SetWeatherSampleGT(
      WeatherLayer weatherLayer,
      ISampleRef weatherSample,
      GameTime timeToBlend)
    {
      IWeatherLayerBlenderItem layerBlenderItem = ServiceLocator.GetService<IWeatherController>().GetItem(weatherLayer);
      if (layerBlenderItem == null)
      {
        Logger.AddError(string.Format("Invalid weather layer : {0}", (object) weatherLayer));
      }
      else
      {
        IWeatherSnapshot template = ServiceCache.TemplateService.GetTemplate<IWeatherSnapshot>(weatherSample.EngineTemplateGuid);
        if (template == null)
          Logger.AddError(string.Format("Weather snapshot with guid={0} not found!", (object) weatherSample.EngineTemplateGuid));
        else
          layerBlenderItem.Blender.BlendTo(template, (TimeSpan) timeToBlend);
      }
    }

    public static VMWeather Instance { get; private set; }
  }
}
