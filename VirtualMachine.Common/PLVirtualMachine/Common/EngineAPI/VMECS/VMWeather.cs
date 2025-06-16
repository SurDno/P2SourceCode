// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMWeather
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Weather;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Weather", null)]
  public class VMWeather : VMComponent
  {
    public const string ComponentName = "Weather";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Method("Set weather layer weight", "Weather layer,Weight,Time to blend", "")]
    public virtual void SetWeatherLayerWeight(
      WeatherLayer weatherLayer,
      float weight,
      float timeToBlend)
    {
    }

    [Method("Set weather layer weight in gametime", "Weather layer,Weight,Game time to blend", "")]
    public virtual void SetWeatherLayerWeightGT(
      WeatherLayer weatherLayer,
      float weight,
      GameTime timeToBlend)
    {
    }

    [Method("Set weather template", "Weather layer,Wather snapshot sample:ISnapshot,Time to blend", "")]
    public virtual void SetWeatherSample(
      WeatherLayer weatherLayer,
      ISampleRef weatherSample,
      float timeToBlend)
    {
    }

    [Method("Set weather template in gametime", "Weather layer,Weather snapshot sample:ISnapshot,Game time to blend", "")]
    public virtual void SetWeatherSampleGT(
      WeatherLayer weatherLayer,
      ISampleRef weatherSample,
      GameTime timeToBlend)
    {
    }
  }
}
